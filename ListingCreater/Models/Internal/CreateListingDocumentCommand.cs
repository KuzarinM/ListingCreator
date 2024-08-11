using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingCreater.Models.Internal
{
    public class CreateListingDocumentCommand: IRequest
    {
        public string DestonationFilename { get; set; }
    }
}
