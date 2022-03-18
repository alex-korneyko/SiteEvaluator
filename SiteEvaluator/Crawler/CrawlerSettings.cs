using System;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.Crawler
{
    public class CrawlerSettings
    {
        public bool IncludeNofollowLinks { get; set; }
        public Action<StringLoadResult>? CrawlHtmlLoadedEvent { get; set; }
        public Action<ImageLoadResult>? CrawlImageLoadedEvent { get; set; }
        public bool LoadMedia { get; set; }
    }
}