using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace ListingCreater.Logic.Handlers
{
    public class SaveCurrentConfigCommandHandler : IRequestHandler<SaveCurrentConfigCommand>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public SaveCurrentConfigCommandHandler(ILogger<SaveCurrentConfigCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public async Task Handle(SaveCurrentConfigCommand request, CancellationToken cancellationToken)
        {
            var configuration = _configurationStorage.GetConfiguration(null);

            if (configuration == null)
                throw new Exception("Конфигурация не найдена");

            int cc = configuration.ColumnsCount;
            int.TryParse(request.ColumnsCountText.Text, out cc);
            configuration.ColumnsCount = cc;

            int tiS = configuration.ListingTitleTextSize;
            int.TryParse(request.TitleSizeText.Text, out tiS);
            configuration.ListingTitleTextSize = tiS;

            int tS = configuration.ListingTextSize;
            int.TryParse(request.TextSizeText.Text, out tS);
            configuration.ListingTextSize = tiS;

            configuration.OutputTabAndReturns = !request.TabRemoveBox.IsChecked.Value;
        }
    }
}
