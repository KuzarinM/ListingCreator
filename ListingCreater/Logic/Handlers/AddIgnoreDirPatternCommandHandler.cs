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
    public class AddIgnoreDirPatternCommandHandler: IRequestHandler<AddIgnoreDirPatternCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public AddIgnoreDirPatternCommandHandler(ILogger<AddIgnoreDirPatternCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public async Task<bool> Handle(AddIgnoreDirPatternCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.IgnoreDirText))
                return false;

            var configuration = _configurationStorage.GetConfiguration(null);
            if (configuration == null)
                return false;

            if (!configuration.IgnoreFolder.Any(x => x == request.IgnoreDirText))
            {
                request.IgnoreDirList.Items.Add(request.IgnoreDirText);
                request.IgnoreDirInput.Text = "";
                configuration.IgnoreFolder.Add(request.IgnoreDirText);
            }
            return true;
        }
    }
}
