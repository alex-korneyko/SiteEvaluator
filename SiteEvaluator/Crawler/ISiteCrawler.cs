using SiteEvaluator.PageLoader;

namespace SiteEvaluator.Crawler;

public interface ISiteCrawler
{
    Task<IList<PageLoadResult>> CrawlAsync(string hostUrl);
}