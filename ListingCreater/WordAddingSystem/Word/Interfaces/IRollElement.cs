using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Interfaces
{
	public interface IRollElement
	{
		public int? RollLavel { get; set; }
		//todo ERROR По идеи, это пустой интерфейс и нужен он лишь для того, чтобы можно было удобно создавать
		//многоуровневые списки. Слово List в навазнии заменено на Roll чтобы не создавать путаницы с List как отчётом в иде списка
	}
}
