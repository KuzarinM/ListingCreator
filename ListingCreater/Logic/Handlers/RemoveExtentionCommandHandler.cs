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
    public class RemoveExtentionCommandHandler: IRequestHandler<RemoveExtentionCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public RemoveExtentionCommandHandler(ILogger<RemoveExtentionCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public async Task<bool> Handle(RemoveExtentionCommand request, CancellationToken cancellationToken)
        {
            if (request.SelectedItem == null)
                return false;

            var configuration = _configurationStorage.GetConfiguration(null);
            if (configuration == null)
                return false;

            var extentionText = request.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(extentionText))
                return false;

            request.ExtentionList.Items.Remove(request.SelectedItem);

            if (configuration.Extentions.Any(x => x == extentionText))
                configuration.Extentions.Remove(extentionText);

            return true;
        }
    }
}
