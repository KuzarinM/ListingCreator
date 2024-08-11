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

                List<(string,WordTextProperties?)> texts = new();
                if (info.OutputTabAndReturns)
                {
                    texts = File.ReadAllLines(item, GetFileEncoding(item)).Select(run => ($"{run}\n", (WordTextProperties?)null)).ToList();
                }
                else
                {
                    texts = new() { (File.ReadAllText(item, GetFileEncoding(item)).Replace("\t", "").Replace("\n", ""), null) };
                }
				
/*                var texts = info.OutputTabAndReturns? File.ReadAllLines(item).Select(run => ($"{run}\n", (WordTextProperties?)null)).ToList()
					: new() {(File.ReadAllText(item).Replace("\t","").Replace("\n",""),null)};*/
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

		public Encoding GetFileEncoding(string filePath)
		{
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            BinaryReader instr = new BinaryReader(File.OpenRead(filePath));
            byte[] data = instr.ReadBytes((int)instr.BaseStream.Length);
            instr.Close();

            // определяем BOM (EF BB BF)
            if (data.Length > 2 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf)
            {
                if (data.Length != 3) return Encoding.UTF8;
                else return Encoding.Default;
            }

            int i = 0;
            while (i < data.Length - 1)
            {
                if (data[i] > 0x7f)
                { // не ANSI-символ
                    if ((data[i] >> 5) == 6)
                    {
                        if ((i > data.Length - 2) || ((data[i + 1] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i++;
                    }
                    else if ((data[i] >> 4) == 14)
                    {
                        if ((i > data.Length - 3) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 2;
                    }
                    else if ((data[i] >> 3) == 30)
                    {
                        if ((i > data.Length - 4) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2) || ((data[i + 3] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 3;
                    }
                    else
                    {
                        return Encoding.GetEncoding(1251);
                    }
                }
                i++;
            }

            return Encoding.UTF8;
        }

		public abstract WordTextProperties DefaultTextProperies { get; }

		public abstract void CreateWord();

		public abstract void CreateParagraph(WordParagraph paragraph);

		public abstract void CreateMarkeredList(MarkeredList list);

		public abstract MemoryStream SaveWord();
	}
}
