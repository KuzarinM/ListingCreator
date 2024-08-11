using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class SetProjectDirCommand: IRequest<string>
    {
        public CommonOpenFileDialog SelectDialog { get; set; } = null!;

        public TextBox ProjectFolderInput { get; set; } = null!;
    }
}
