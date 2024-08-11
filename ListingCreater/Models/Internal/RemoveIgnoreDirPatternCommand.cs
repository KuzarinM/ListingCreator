using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ListingCreater.Models.Internal
{
    public class RemoveIgnoreDirPatternCommand:IRequest<bool>
    {
        public object? SelectedItem { get; set; } = null!;

        public ListBox IgnoreDirList { get; set; } = null!;
    }
}
