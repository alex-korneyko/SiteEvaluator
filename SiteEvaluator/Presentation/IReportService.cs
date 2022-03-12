using System.Collections.Generic;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Presentation
{
    public interface IReportService
    {
        void AddCrawlerResults(IEnumerable<ContentLoadResult> crawlerResults);
        void AddSiteMapExplorerResults(IEnumerable<ContentLoadResult> siteMapExploreResults);
        IEnumerable<ContentLoadResult> GetOnlyInSiteMapResults();
        IEnumerable<ContentLoadResult> GetOnlyInCrawlerResults();
        IEnumerable<ContentLoadResult> GetCompositeReport();
    }
}