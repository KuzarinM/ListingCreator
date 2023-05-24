using ListingAutoCreater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListingCreater.Logic
{
    public class FileFinder
    {
        public List<string> GetAllFileInFolder(FileSearchModel model)
        {
            List<string> result = new List<string>();
            GetAllFiles(model.FolderPath, model.RequredExtentions, model.IgnoredFolderName, result);
            return result;
        }

        private void GetAllFiles(string folderPath, List<string> extentions, List<string> ignoreFolder, List<string> resout)
        {
            foreach (string ex in extentions)
            {
                resout.AddRange(Directory.GetFiles(folderPath, $"*{ex}"));
            }
            foreach (var subDir in Directory.GetDirectories(folderPath))
            {
                if (!ignoreFolder.Any(x => Regex.IsMatch(subDir, $"{x}$")))
                {
                    GetAllFiles(subDir, extentions, ignoreFolder, resout);
                }
            }
        }
    }
}
