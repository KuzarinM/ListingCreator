using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingAutoCreater.Models
{
    public class FileSearchModel
    {
        public string FolderPath { get; set; } = string.Empty;

        public List<string> RequredExtentions { get; set; } = new();

        public List<string> IgnoredFolderName { get; set; } = new();

    }
}
