using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ListingCreater.Logic.Handlers
{
    public class GetProjectRequredFilenamesQueryHandler : IRequestHandler<GetProjectRequredFilenamesQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;

        public GetProjectRequredFilenamesQueryHandler(ILogger<GetProjectRequredFilenamesQueryHandler> logger, IConfigurationStorage configurationStorage)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
        }

        public Task<List<string>> Handle(GetProjectRequredFilenamesQuery request, CancellationToken cancellationToken)
        {
            var res = new List<string>();

            var config = _configurationStorage.GetConfiguration(null);

            if (config == null)
                throw new Exception("Конфигурация не найдёна");

            GetAllFiles(config.ProjectDirectoryName, config.Extentions, config.IgnoreFolder, res);

            return Task.FromResult(res);
        }

        private void GetAllFiles(string folderPath, List<string> extentions, List<string> ignoreFolder, List<string> resout)
        {
            foreach (string ex in extentions)
            {
                resout.AddRange(Directory.GetFiles(folderPath, $"*{ex}"));
            }
            foreach (var subDir in Directory.GetDirectories(folderPath))
            {
                if (!ignoreFolder.Any(x => Regex.IsMatch(subDir, $"{x}$")))
                {
                    GetAllFiles(subDir, extentions, ignoreFolder, resout);
                }
            }
        }
    }
}
