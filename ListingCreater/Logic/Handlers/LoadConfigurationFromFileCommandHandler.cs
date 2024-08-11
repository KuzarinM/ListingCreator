using ListingCreater.Models;
using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ListingCreater.Logic.Handlers
{
    public class LoadConfigurationFromFileCommandHandler: IRequestHandler<LoadConfigurationFromFileCommand>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public LoadConfigurationFromFileCommandHandler(ILogger<LoadConfigurationFromFileCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public Task Handle(LoadConfigurationFromFileCommand request, CancellationToken cancellationToken)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ListingConfiguration));

            using (FileStream fs = new FileStream(request.Filename, FileMode.Open))
            {
                var config = xmlSerializer.Deserialize(fs) as ListingConfiguration;

                if (config == null)
                    throw new Exception("Не удалось загрузить конфигурацию");

                _configurationStorage.SaveConfiguration(config);

                request.ExtentionList.Items.Clear();
                config.Extentions.ForEach(x=>request.ExtentionList.Items.Add(x));

                request.IgnoreList.Items.Clear();
                config.IgnoreFolder.ForEach(x => request.IgnoreList.Items.Add(x));

                request.TextSizeText.Text = config.ListingTextSize.ToString();
                request.TitleSizeText.Text = config.ListingTitleTextSize.ToString();
                request.ColumnsCountText.Text = config.ColumnsCount.ToString();
                request.TabRemoveBox.IsChecked = !config.OutputTabAndReturns;

            }

            return Task.CompletedTask;
        }
    }
}
