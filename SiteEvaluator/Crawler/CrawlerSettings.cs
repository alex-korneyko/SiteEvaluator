using System;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Crawler
{
    public class CrawlerSettings
    {
        public bool IncludeNofollowLinks { get; set; }
        public Action<ContentLoadResult>? CrawlEvent { get; set; }
    }
}