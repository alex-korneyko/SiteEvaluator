using System;
using System.Collections.Generic;
using SiteEvaluator.Data;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator
{
    public class PageInfo: IEquatable<PageInfo>, IHasContent
    {
        public string Url { get; }
        public string Content { get; set; }
        public IList<string> InnerUrls { get; set; } = new List<string>();
        public IList<string> ExternalUrls { get; set; } = new List<string>();
        public IList<string> MediaUrls { get; set; } = new List<string>();
        public long TotalLoadTime { get; set; }
        public int Level { get; set; }
        public long TotalSize { get; set; }

        public PageInfo()
        {
        }

        public PageInfo(StringLoadResult stringLoadResult, int level = 0)
        {
            Url = stringLoadResult.PageUrl;
            Content = stringLoadResult.Content ?? string.Empty;
            TotalLoadTime += stringLoadResult.ContentLoadTime;
            TotalSize += stringLoadResult.Size;
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
    }
}