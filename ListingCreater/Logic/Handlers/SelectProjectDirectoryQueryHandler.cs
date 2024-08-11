using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ListingCreater.Logic.Handlers
{
    public class SelectProjectDirectoryQueryHandler : IRequestHandler<SelectProjectDirectoryQuery, string?>
    {
        private readonly ILogger _logger;

        public SelectProjectDirectoryQueryHandler(ILogger<SelectProjectDirectoryQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<string?> Handle(SelectProjectDirectoryQuery request, CancellationToken cancellationToken)
        {
            if(!request.SelectDialog.IsFolderPicker)
                request.SelectDialog.IsFolderPicker = true;

            if(!string.IsNullOrEmpty(request.InitialDirectory))
                request.SelectDialog.InitialDirectory = request.InitialDirectory;

            var res = request.SelectDialog.ShowDialog();
            if(res == CommonFileDialogResult.Ok)
            {
                return request.SelectDialog.FileName;
            }
            return null;
        }
    }
}
