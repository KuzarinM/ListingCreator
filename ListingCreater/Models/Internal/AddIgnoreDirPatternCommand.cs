using ListingCreater.Logic.Handlers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class AddIgnoreDirPatternCommand:IRequest<bool>
    {
        public string IgnoreDirText { get; set; } = null!;

        public ListBox IgnoreDirList { get; set; } = null!;

        public TextBox IgnoreDirInput { get; set; } = null!;
    }
}
