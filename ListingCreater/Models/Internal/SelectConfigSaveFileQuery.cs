using MediatR;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingCreater.Models.Internal
{
    public class SelectConfigSaveFileQuery: IRequest<string?>
    {
        public FileDialog SaveFileDialog { get; set; }
    }
}
