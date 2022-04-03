using System;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Crawler
{
    public interface ISiteCrawler
    {
        Task<TargetHost> CrawlAsync(Uri hostUri, Action<CrawlerSettings>? crawlerSettings = null);
    }
}