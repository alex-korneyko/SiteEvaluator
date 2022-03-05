using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Crawler
{
    public interface ISiteCrawler
    {
        Task<IList<ContentLoadResult>> CrawlAsync(string hostUrl);
    }
}