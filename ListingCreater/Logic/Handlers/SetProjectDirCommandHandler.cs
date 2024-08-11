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
    public class SetProjectDirCommandHandler : IRequestHandler<SetProjectDirCommand, string>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;
        private readonly IMediator _mediator;

        public SetProjectDirCommandHandler(ILogger<SetProjectDirCommandHandler> logger, IConfigurationStorage configurationStorage, IMediator mediator)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
            _mediator = mediator;
        }

        public async Task<string> Handle(SetProjectDirCommand request, CancellationToken cancellationToken)
        {
            var configuration = _configurationStorage.GetConfiguration(null);
            if (configuration == null)
                return string.Empty;

            var res = await _mediator.Send(new SelectProjectDirectoryQuery()
            {
                SelectDialog = request.SelectDialog,
                InitialDirectory = configuration.ProjectDirectoryName,
            });

            if(res == null)
                return string.Empty;

            configuration.ProjectDirectoryName = res;
            request.ProjectFolderInput.Text = res;
            return res;
        }
    }
}
