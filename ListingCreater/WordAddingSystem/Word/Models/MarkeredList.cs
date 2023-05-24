using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models
{
	public class MarkeredList : IRollElement
	{
		public List<IRollElement> childs { get; set; } = new();

		public int? RollLavel { get; set; }
	}
}
