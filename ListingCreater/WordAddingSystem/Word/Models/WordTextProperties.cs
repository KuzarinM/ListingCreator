using ListingCreater.WordAddingSystem.Word.Models;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models
{
	public class WordTextProperties
	{
		public int? Size { get; set; } = null;

		public bool? Bold { get; set; } = null;

		public WordJustificationType? JustificationType { get; set; } = null;

		public int ColumnCount { get; set; } = 1;

		public int BetweenColumnSpace { get; set; } = 0;

		public WordPageMargin? PageMagin {  get; set; }
    }
}
