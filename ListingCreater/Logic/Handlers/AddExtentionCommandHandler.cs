using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListingCreater.Logic.Handlers
{
    public class AddExtentionCommandHandler : IRequestHandler<AddExtentionCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public AddExtentionCommandHandler(ILogger<AddExtentionCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public async Task<bool> Handle(AddExtentionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ExtentionText))
                return false;

            var configuration = _configurationStorage.GetConfiguration(null);
            if (configuration == null)
                return false;

            if (!request.ExtentionText.StartsWith("."))
                request.ExtentionText = $".{request.ExtentionText}";

            if (!configuration.Extentions.Any(x => x == request.ExtentionText))
            {
                request.ExtentionList.Items.Add(request.ExtentionText);
                request.ExtentionInput.Text = "";
                configuration.Extentions.Add(request.ExtentionText);
            }
            return true;
        }
    }
}
