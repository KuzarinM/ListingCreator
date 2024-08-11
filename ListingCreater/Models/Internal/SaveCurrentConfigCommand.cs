using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class SaveCurrentConfigCommand : IRequest
    {
        public TextBox ColumnsCountText {  get; set; }

        public TextBox TitleSizeText { get; set; }

        public TextBox TextSizeText { get; set; }

        public CheckBox TabRemoveBox {  get; set; }
    }
}
