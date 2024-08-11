using ListingCreater.Models;
using ListingCreater.Storages.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ListingCreater.Storages.Implementations
{
    public class ConfigurationStorage: IConfigurationStorage
    {
        private readonly ILogger _logger;
        private ListingConfiguration _listingConfiguration;

        public ConfigurationStorage(ILogger<ConfigurationStorage> logger) 
        {
            _logger = logger;
            _listingConfiguration = new();
        }

        public ListingConfiguration? GetConfiguration(string? Name)
        {
            return _listingConfiguration;
        }

        public bool SaveConfiguration(ListingConfiguration configuration)
        {
            _listingConfiguration = configuration;

            return true;
        }
    }
}
