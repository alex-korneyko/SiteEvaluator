using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.PageLoader;

namespace SiteEvaluator.Crawler
{
    public interface ISiteCrawler
    {
        Task<IList<ContentLoadResult>> CrawlAsync(string hostUrl);
    }
}