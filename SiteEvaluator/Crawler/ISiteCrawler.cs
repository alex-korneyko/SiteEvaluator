using SiteEvaluator.PageLoader;

namespace SiteEvaluator.Crawler;

public interface ISiteCrawler
{
    Task<IList<ContentLoadResult>> CrawlAsync(string hostUrl);
}