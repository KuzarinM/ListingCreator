using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class AddExtentionCommand : IRequest<bool>
    {
        public string ExtentionText {  get; set; } = null!;

        public ListBox ExtentionList { get; set; } = null!;

        public TextBox ExtentionInput { get; set; } = null!;
    }
}
