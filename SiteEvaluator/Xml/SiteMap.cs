using System.Xml.Serialization;

namespace SiteEvaluator.Xml
{
    [XmlRoot("urlset")]
    public class SiteMap
    {
        [XmlElement("url")] public Url[]? UrlSet { get; set; }
    }
}