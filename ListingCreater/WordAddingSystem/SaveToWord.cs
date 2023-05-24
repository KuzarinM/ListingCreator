using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ServiceStationBusinessLogic.OfficePackage.HelperModels;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Enums;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage.Implements
{
	public class SaveToWord : AbstractSaveToWord
	{
		private WordprocessingDocument? _wordDocument;
		protected NumberingDefinitionsPart? _numberingPart;//Тут храняться списки(Заполняется только по необходиомсти)
		private Body? _docBody;
		private MemoryStream? stream;

		#region Служебные методы, необходимые для записи в word документ
		private static JustificationValues GetJustificationValues(WordJustificationType type) => type switch
		{
			WordJustificationType.Both => JustificationValues.Both,
			WordJustificationType.Center => JustificationValues.Center,
			WordJustificationType.Left => JustificationValues.Left,
			_ => JustificationValues.Left,
		};

		private static SectionProperties CreateSectionProperties()
		{
			return new SectionProperties(new PageSize
			{
				Orient = PageOrientationValues.Portrait
			});
		}

		private static ParagraphProperties? CreateParagraphProperties(WordTextProperties? paragraphProperties)
		{
			if (paragraphProperties == null)
			{
				return null;
			}

			var properties = new ParagraphProperties();
			if (paragraphProperties.JustificationType.HasValue)
			{
				properties.AppendChild(new Justification()
				{
					Val = GetJustificationValues(paragraphProperties.JustificationType.Value)
				});
			}

            properties.AppendChild(new Columns()
            {
                EqualWidth = true,
                ColumnCount = (Int16Value)paragraphProperties.ColumnCount,
            });

            properties.AppendChild(new SpacingBetweenLines
			{
				LineRule = LineSpacingRuleValues.Auto
			});

			properties.AppendChild(new Indentation());

			var paragraphMarkRunProperties = new ParagraphMarkRunProperties();
			if (paragraphProperties.Size.HasValue)
			{
				paragraphMarkRunProperties.AppendChild(new FontSize { Val = (paragraphProperties.Size.Value * 2).ToString() });//todo ERROR По неизвесной науке причине 10 шрифт в коде, это 5 в word.
			}
			properties.AppendChild(paragraphMarkRunProperties);

			return properties;
		}

		private Paragraph? MakeParagraph(WordParagraph paragraph)
		{
			var docParagraph = new Paragraph();

			docParagraph.AppendChild(CreateParagraphProperties(paragraph.TextProperties));

			foreach (var run in paragraph.Texts)
			{
				if (string.IsNullOrEmpty(run.run) || Regex.IsMatch(run.run,@"^(\s|\t|\n)+$"))
					continue;
				var docRun = new Run();
				var properties = new RunProperties();

				//todo WARNING По несовсем ясной причине, настройки шрифтов не применяются к прогонам параграфа. Поэтому, их необходимо выставлять вручную.
				if (run.properties != null && run.properties.Size.HasValue)
				{
					properties.AppendChild(new FontSize { Val = (run.properties.Size.Value * 2).ToString() });
				}
				else if (paragraph.TextProperties != null && paragraph.TextProperties.Size.HasValue)
				{
					properties.AppendChild(new FontSize { Val = (paragraph.TextProperties.Size.Value * 2).ToString() });
				}
				if (run.properties != null && run.properties.Bold.HasValue && run.properties.Bold.Value ||
					paragraph.TextProperties != null && paragraph.TextProperties.Bold.HasValue && paragraph.TextProperties.Bold.Value)
				{
					properties.AppendChild(new Bold());
				}

				docRun.AppendChild(properties);

				docRun.AppendChild(new Text { Text = run.Item1, Space = SpaceProcessingModeValues.Preserve });

				if (run.run.EndsWith("\n"))
				{
                    docRun.AppendChild(new Break());
                }

				docParagraph.AppendChild(docRun);
			}
			return docParagraph;
		}

		private void InitLists()
		{
			if (_wordDocument == null)
			{
				throw new InvalidOperationException("Сначала создайте документ!");
			}
			_numberingPart = _wordDocument.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("documentsLIsts");//Ну, тут не может она null быть, так как иначе это совсем что-то странное

			Level firstLevel = new Level(
				new NumberingFormat() { Val = NumberFormatValues.Decimal },
				new LevelText() { Val = "%1." }
				)
			{
				LevelIndex = 0,
				StartNumberingValue = new StartNumberingValue() { Val = 1 }
			};
			Level secondLevel = new Level(
				new NumberingFormat() { Val = NumberFormatValues.Bullet },
				new LevelText() { Val = "●" }
				)
			{
				LevelIndex = 1,
			};
			Level thridLavel = new Level(
				new NumberingFormat() { Val = NumberFormatValues.Bullet }
				)
			{
				LevelIndex = 2,
			};
			//todo INFO Иного сопособа сделать списки Нормальными(с отступами) не существует. openXML сам такое не умеет
			//Все данные вязяти из документа word(созданного силами самого редактора word) со списками(пришлось лесть в xml структуру).
			firstLevel.AppendChild(new ParagraphProperties(new Indentation
			{
				Left = "720",
				Hanging = "360"
			}));
			secondLevel.AppendChild(new ParagraphProperties(new Indentation
			{
				Left = "1440",
				Hanging = "360"
			}));
			thridLavel.AppendChild(new ParagraphProperties(new Indentation
			{
				Left = "2160",
				Hanging = "360"
			}));

			var num = new AbstractNum(firstLevel, secondLevel, thridLavel);
			num.AbstractNumberId = 1;

			var element = new Numbering(num, new NumberingInstance(new AbstractNumId { Val = 1 }) { NumberID = 1 });
			element.Save(_numberingPart);
		}

		private void MakeList(MarkeredList list, int level = 0)
		{
			if (_docBody == null)
				return;

			int listId = 1;

			foreach (var line in list.childs)
			{
				if (line is WordParagraph pLine)
				{
					var paragraph = MakeParagraph(pLine);
					if (paragraph == null)
						continue;
					var numProp = new NumberingProperties(
						new NumberingLevelReference() { Val = line.RollLavel.HasValue? line.RollLavel : level },
						new NumberingId() { Val = listId });;
					if (paragraph.ParagraphProperties == null)
					{
						throw new ArgumentNullException("This situation is imposible, if you create paragraf with TextProperties properties");
					}
					paragraph.ParagraphProperties.AppendChild(numProp);
					_docBody.AppendChild(paragraph);
				}
				else if (line is MarkeredList mList)
				{
					MakeList(mList, level + 1);
				}
			}

		}

		#endregion

		protected override WordTextProperties DefaultTextProperies => new()
		{
			Size = 14,
			Bold = false,
			JustificationType = WordJustificationType.Both
		};

		protected override void CreateWord()
		{
			stream = new MemoryStream();

			_wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
			MainDocumentPart mainPart = _wordDocument.AddMainDocumentPart();
			mainPart.Document = new Document();
			_docBody = mainPart.Document.AppendChild(new Body());
		}

		protected override void CreateMarkeredList(MarkeredList list)
		{
			if (_docBody == null)
			{
				throw new InvalidOperationException("Сначала создайте документ");
			}
			if (_numberingPart == null)
			{
				InitLists();
			}
			MakeList(list);
		}

		protected override void CreateParagraph(WordParagraph paragraph)
		{
			if (_docBody == null || paragraph == null)
			{
				return;
			}
			var docParagraph = MakeParagraph(paragraph);
			if (paragraph == null)
				return;
			_docBody.AppendChild(docParagraph);

		}

		protected override MemoryStream SaveWord()
		{
			if (_docBody == null || _wordDocument == null)
			{
				throw new InvalidOperationException("To save a document, it must first be created!");
			}
			_docBody.AppendChild(CreateSectionProperties());

			_wordDocument.MainDocumentPart!.Document.Save();

			_wordDocument.Dispose();

			if (stream == null)
				throw new InvalidOperationException("The return stream was empty!");

			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

	}
}
