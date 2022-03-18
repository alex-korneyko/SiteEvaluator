using System;
using System.Xml.Serialization;

namespace SiteEvaluator.SiteMapExploring.Parser
{
    public class Url : IEquatable<Url>
    {
        [XmlElement("loc")]
        public string? Loc { get; set; }

        [XmlElement("lastmod")]
        public DateTime? LastMod { get; set; }

        [XmlElement("priority")]
        public float? Priority { get; set; }

        [XmlElement("changefreq")]
        public string? ChangeFreq { get; set; }

        public bool Equals(Url? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Loc == other.Loc && Nullable.Equals(LastMod, other.LastMod) && Nullable.Equals(Priority, other.Priority) && ChangeFreq == other.ChangeFreq;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Url)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Loc, LastMod, Priority, ChangeFreq);
        }
    }
}