using MediatR;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingCreater.Models.Internal
{
    public class SelectListingDestonationQuery: IRequest<string?>
    {
        public string? InitialDirectory { get; set; } = null!;
        public SaveFileDialog SelectDialog { get; set; } = null!;
    }
}
