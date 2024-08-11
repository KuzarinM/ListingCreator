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
    public class SaveConfigurationToFileCommandHandler : IRequestHandler<SaveConfigurationToFileCommand>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public SaveConfigurationToFileCommandHandler(ILogger<SaveConfigurationToFileCommandHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public Task Handle(SaveConfigurationToFileCommand request, CancellationToken cancellationToken)
        {
            var config = _configurationStorage.GetConfiguration(null);

            if (config == null)
                throw new Exception("Ошибка при получении конфига");


            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ListingConfiguration));

            using (FileStream fs = new FileStream(request.Filepath, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, config);
            }

            return Task.CompletedTask;
        }
    }
}
