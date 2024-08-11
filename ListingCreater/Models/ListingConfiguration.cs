using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ListingCreater.Models
{
    public class ListingConfiguration
    {
        public int version { get; set; } = 1;

        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public string ProjectDirectoryName { get; set; } = string.Empty;

        public List<string> Extentions { get; set; } = new();

        public List<string> IgnoreFolder { get; set; } = new();

        public bool OutputTabAndReturns = false;

        public int ColumnsCount { get; set; } = 2;

        public int ListingTextSize { get; set; } = 8;

        public int ListingTitleTextSize { get; set; } = 9;

        public XElement GetXElement => new("Settings",
            new XElement("ProjectDirectoryName", ProjectDirectoryName),
            new XElement("Extentions", Extentions.Select(x => new XElement("Element", x)).ToList()),
            new XElement("IgnoreFolder", IgnoreFolder.Select(x => new XElement("Element", x)).ToList()),
            new XElement("OutputTabAndReturns", OutputTabAndReturns),
            new XElement("ColumnsCount", ColumnsCount),
            new XElement("ListingTextSize", ListingTextSize),
            new XElement("ListingTitleTextSoze", ListingTitleTextSize));

        public static ListingConfiguration Create(XElement element) => new ListingConfiguration()
        {
            ProjectDirectoryName = element.Element("Settings")!.Element("ProjectDirectoryName")!.Value,
            Extentions = element.Element("Settings")!.Element("Extentions")!.Elements("Element").Select(x => x.Value).ToList(),
            IgnoreFolder = element.Element("Settings")!.Element("IgnoreFolder")!.Elements("Element").Select(x => x.Value).ToList(),
            OutputTabAndReturns = Convert.ToBoolean(element.Element("Settings")!.Element("OutputTabAndReturns")!.Value),
            ColumnsCount = Convert.ToInt32(element.Element("Settings")!.Element("ColumnsCount")!.Value),
            ListingTextSize = Convert.ToInt32(element.Element("Settings")!.Element("ListingTextSize")!.Value),
            ListingTitleTextSize = Convert.ToInt32(element.Element("Settings")!.Element("ListingTitleTextSoze")!.Value)
        };
    }
}
