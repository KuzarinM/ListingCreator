using ListingCreater.Models;
using ListingCreater.Models.Internal;
using ListingCreater.Storages.Implementations;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListingCreater.Logic.Handlers
{
    public class SelectConfigSaveFileQueryHandler : IRequestHandler<SelectConfigSaveFileQuery, string?>
    {
        private readonly ILogger _logger;

        public SelectConfigSaveFileQueryHandler(ILogger<SelectConfigSaveFileQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<string?> Handle(SelectConfigSaveFileQuery request, CancellationToken cancellationToken)
        {
            request.SaveFileDialog.FileName = "Конфигурация"; // Default file name
            request.SaveFileDialog.DefaultExt = ".xml"; // Default file extension
            request.SaveFileDialog.Filter = "XML Document (.xml)|*.xml"; // Filter files by extension

            var res = request.SaveFileDialog.ShowDialog();
            if (res != null && res.Value)
            {
                return Task.FromResult<string?>(request.SaveFileDialog.FileName);
            }
            return Task.FromResult<string?>(null);
        }
    }
}
