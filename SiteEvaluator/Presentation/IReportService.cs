using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Presentation
{
    public interface IReportService
    {
        // Task AddCrawlerResultsAsync(Uri hostUri, IEnumerable<PageInfo> crawlerResults);
        // Task AddSiteMapExplorerResultsAsync(Uri hostUri, IEnumerable<PageInfo> siteMapExploreResults);
        Task<IEnumerable<PageInfo>> GetCrawlerResultsAsync(Uri hostUri);
        Task<IEnumerable<PageInfo>> GetSiteMapResultsAsync(Uri hostUri);
        Task<IEnumerable<PageInfo>> GetUniqInSiteMapResults(Uri hostUri);
        Task<IEnumerable<PageInfo>> GetUniqCrawlerResults(Uri hostUri);
        Task<IEnumerable<PageInfo>> GetCompositeReportAsync(Uri hostUri);
    }
}