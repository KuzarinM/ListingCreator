using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models
{
	public class WordParagraph : IRollElement
	{
		public List<(string run, WordTextProperties? properties)> Texts { get; set; } = new();

		public WordTextProperties? TextProperties { get; set; }

		public int? RollLavel { get; set; }
	}
}
