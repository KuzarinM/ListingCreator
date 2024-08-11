using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class LoadConfigurationFromFileCommand: IRequest
    {
        public string Filename { get; set; }

        public ListBox ExtentionList { get; set; }

        public ListBox IgnoreList { get; set; }

        public TextBox ColumnsCountText { get; set; }

        public TextBox TitleSizeText { get; set; }

        public TextBox TextSizeText { get; set; }

        public CheckBox TabRemoveBox { get; set; }
    }
}
