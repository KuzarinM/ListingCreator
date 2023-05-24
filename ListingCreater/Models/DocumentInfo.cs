using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingAutoCreater.Models
{
    public class DocumentInfo
    {
        public string FileName { get; set; } = string.Empty;

        public List<string> Files { get; set; } = new();

        public string HomeProjectDirectory { get; set; } = string.Empty;

        public bool OutputTabAndReturns = false;

        public int ColumnsCount = 3;

        public int ListingTextSize = 6;

        public int ListingTitleTextSoze = 7;
    }
}
