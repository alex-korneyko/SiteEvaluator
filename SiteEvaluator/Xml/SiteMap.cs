using System.Xml.Serialization;

namespace SiteEvaluator.Xml;

[XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class SiteMap
{
    [XmlElement("url")]
    public Url[]? UrlSet { get; set; }
}