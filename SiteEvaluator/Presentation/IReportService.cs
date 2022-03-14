using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Presentation
{
    public interface IReportService
    {
        Task AddCrawlerResultsAsync(string hostUrl, IEnumerable<ContentLoadResult> crawlerResults);
        Task AddSiteMapExplorerResultsAsync(string hostUrl, IEnumerable<ContentLoadResult> siteMapExploreResults);
        Task<IEnumerable<ContentLoadResult>> GetCrawlerResultsAsync(string hostUrl);
        Task<IEnumerable<ContentLoadResult>> GetSiteMapResultsAsync(string hostUrl);
        Task<IEnumerable<ContentLoadResult>> GetUniqInSiteMapResults(string hostUrl);
        Task<IEnumerable<ContentLoadResult>> GetUniqCrawlerResults(string hostUrl);
        Task<IEnumerable<ContentLoadResult>> GetCompositeReportAsync(string hostUrl);
    }
}