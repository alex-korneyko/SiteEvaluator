using System;
using System.Collections.Generic;
using SiteEvaluator.Data;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator
{
    public class PageInfo: IEquatable<PageInfo>, IComparable<PageInfo>, IHasContent
    {
        public string Url { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public IList<string> InnerUrls { get; set; } = new List<string>();
        public IList<string> OuterUrls { get; set; } = new List<string>();
        public IList<string> MediaUrls { get; set; } = new List<string>();
        public long TotalLoadTime { get; set; }
        public int Level { get; set; }
        public long TotalSize { get; set; }

        public PageInfo()
        {
        }

        public PageInfo(StringLoadResult stringLoadResult, int level = 0)
        {
            Url = stringLoadResult.RequestedUrl;
            Content = stringLoadResult.Content ?? string.Empty;
            TotalLoadTime = stringLoadResult.ContentLoadTime;
            TotalSize = stringLoadResult.Size;
            Level = level;
        }
        
        public void ClearContent()
        {
            Content = string.Empty;
        }

        public bool Equals(PageInfo? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            
            if (ReferenceEquals(this, other))
                return true;
            
            return Url == other.Url;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            
            if (ReferenceEquals(this, obj))
                return true;
            
            if (obj.GetType() != this.GetType())
                return false;
            
            return Equals((PageInfo)obj);
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }

        public int CompareTo(PageInfo? other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            
            if (ReferenceEquals(null, other))
                return 1;
            
            return TotalLoadTime.CompareTo(other.TotalLoadTime);
        }
    }
}