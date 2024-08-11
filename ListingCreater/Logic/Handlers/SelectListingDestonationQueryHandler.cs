using ListingCreater.Models.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ListingCreater.Logic.Handlers
{
    public class SelectListingDestonationQueryHandler : IRequestHandler<SelectListingDestonationQuery, string?>
    {
        private readonly ILogger _logger;

        public SelectListingDestonationQueryHandler(ILogger<SelectListingDestonationQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<string?> Handle(SelectListingDestonationQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.InitialDirectory))
                request.SelectDialog.InitialDirectory = request.InitialDirectory;

            request.SelectDialog.FileName = "Листинг"; // Default file name
            request.SelectDialog.DefaultExt = ".docx"; // Default file extension
            request.SelectDialog.Filter = "Word Document (.docx)|*.docx"; // Filter files by extension

            var res = request.SelectDialog.ShowDialog();
            if (res!=null && res.Value)
            {
                return request.SelectDialog.FileName;
            }
            return null;
        }
    }
}
