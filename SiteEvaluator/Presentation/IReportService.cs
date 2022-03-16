using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.Presentation
{
    public interface IReportService
    {
        Task AddCrawlerResultsAsync(string hostUrl, IEnumerable<PageInfo> crawlerResults);
        Task AddSiteMapExplorerResultsAsync(string hostUrl, IEnumerable<PageInfo> siteMapExploreResults);
        Task<IEnumerable<PageInfo>> GetCrawlerResultsAsync(string hostUrl);
        Task<IEnumerable<PageInfo>> GetSiteMapResultsAsync(string hostUrl);
        Task<IEnumerable<PageInfo>> GetUniqInSiteMapResults(string hostUrl);
        Task<IEnumerable<PageInfo>> GetUniqCrawlerResults(string hostUrl);
        Task<IEnumerable<PageInfo>> GetCompositeReportAsync(string hostUrl);
    }
}