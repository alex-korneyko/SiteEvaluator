using System;
using System.Collections.Generic;

namespace SiteEvaluator.Data.Model
{
    public class PageInfo: IEquatable<PageInfo>, IComparable<PageInfo>, IHasContent, IEntity
    {
        public int Id { get; set; }
        public string SourceHost { get; set; }
        public ScannerType ScannerType { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<PageInfoUrl> PageInfoUrls { get; set; } = new();
        public long TotalLoadTime { get; set; }
        public int Level { get; set; }
        public long TotalSize { get; set; }

        public PageInfo()
        {
        }

        public PageInfo(IPageInfoLoadResult pageInfoLoadResult, string sourceHost, ScannerType scannerType, int level = 0)
        {
            SourceHost = sourceHost;
            ScannerType = scannerType;
            Url = pageInfoLoadResult.RequestedUrl;
            Content = pageInfoLoadResult.Content ?? string.Empty;
            TotalLoadTime = pageInfoLoadResult.ContentLoadTime;
            TotalSize = pageInfoLoadResult.Size;
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