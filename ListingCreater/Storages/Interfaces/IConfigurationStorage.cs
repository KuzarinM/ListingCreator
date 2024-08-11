using ListingCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingCreater.Storages.Interfaces
{
    public interface IConfigurationStorage
    {
        ListingConfiguration? GetConfiguration(string? Name);

        bool SaveConfiguration(ListingConfiguration configuration);
    }
}
