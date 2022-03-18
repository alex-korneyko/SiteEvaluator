using System.Xml.Serialization;

namespace SiteEvaluator.SiteMapExploring.Parser
{
    [XmlRoot("urlset")]
    public class SiteMap
    {
        [XmlElement("url")] public Url[]? UrlSet { get; set; }
    }
}