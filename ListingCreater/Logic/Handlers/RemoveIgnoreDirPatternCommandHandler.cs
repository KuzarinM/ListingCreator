using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
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
    public class RemoveIgnoreDirPatternCommandHandler : IRequestHandler<RemoveIgnoreDirPatternCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public RemoveIgnoreDirPatternCommandHandler(ILogger<RemoveIgnoreDirPatternCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public async Task<bool> Handle(RemoveIgnoreDirPatternCommand request, CancellationToken cancellationToken)
        {
            if (request.SelectedItem == null)
                return false;

            var configuration = _configurationStorage.GetConfiguration(null);
            if (configuration == null)
                return false;

            var selectedText = request.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selectedText))
                return false;

            request.IgnoreDirList.Items.Remove(request.SelectedItem);

            if (configuration.IgnoreFolder.Any(x => x == selectedText))
                configuration.Extentions.Remove(selectedText);

            return true;
        }
    }
}
