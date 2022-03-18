using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.Crawler
{
    public interface ISiteCrawler
    {
        Task<IList<PageInfo>> CrawlAsync(string hostUrl, Action<CrawlerSettings>? crawlerSettings = null);
    }
}