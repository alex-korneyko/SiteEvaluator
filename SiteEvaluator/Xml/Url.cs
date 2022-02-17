using System.Xml.Serialization;

namespace SiteEvaluator.Xml;

public class Url
{
    [XmlElement("loc")]
    public string? Loc { get; set; }
    
    [XmlElement("lastmod")]
    public DateTime? LastMod { get; set; }
}