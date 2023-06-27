using DocumentFormat.OpenXml.Office2010.Word;
using ListingAutoCreater.Models;
using ServiceStationBusinessLogic.OfficePackage.HelperModels;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Enums;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Interfaces;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage
{
    public abstract class AbstractSaveToWord
	{
		public MemoryStream CreateFaultListDoc(DocumentInfo info)
		{
			CreateWord();

/*			CreateParagraph(new WordParagraph
			{
				Texts = new()
				{
					("Сгенерированный в автоматическом режиме листинг кода системы.",null)
				},
				TextProperties = new()
				{
					Size = 24,
					Bold = true,
					JustificationType = WordJustificationType.Center
				}
			});*/

			foreach (var item in info.Files)
			{
				CreateParagraph(new WordParagraph
				{
					Texts = new()
					{
						(item.Replace(info.HomeProjectDirectory,""),null)
					},
					TextProperties = new()
					{
						Size = info.ListingTitleTextSoze,
						Bold= true,
						JustificationType = WordJustificationType.Both,
                        ColumnCount = info.ColumnsCount
                    }
				});

				var texts = info.OutputTabAndReturns? File.ReadAllLines(item).Select(run => ($"{run}\n", (WordTextProperties?)null)).ToList()
					: new() {(File.ReadAllText(item).Replace("\t","").Replace("\n",""),null)};
                CreateParagraph(new WordParagraph
                {
					Texts = texts,
					TextProperties = new()
                    {
                        Size = info.ListingTextSize,
                        Bold = false,
                        JustificationType = WordJustificationType.Left,
						ColumnCount = info.ColumnsCount
                    }
                });
            }
			
			return SaveWord();
		}

		protected abstract WordTextProperties DefaultTextProperies { get; }

		protected abstract void CreateWord();

		protected abstract void CreateParagraph(WordParagraph paragraph);

		protected abstract void CreateMarkeredList(MarkeredList list);

		protected abstract MemoryStream SaveWord();
	}
}
