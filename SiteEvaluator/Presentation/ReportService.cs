using System.Collections.Generic;
using System.Linq;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Presentation
{
    public class ReportService : IReportService
    {
        private IEnumerable<ContentLoadResult> _crawlerResults = new List<ContentLoadResult>(0);
        private IEnumerable<ContentLoadResult> _siteMapResults = new List<ContentLoadResult>(0);

        public void AddCrawlerResults(IEnumerable<ContentLoadResult> crawlerResults)
        {
            _crawlerResults = crawlerResults;
        }

        public void AddSiteMapExplorerResults(IEnumerable<ContentLoadResult> siteMapExploreResults)
        {
            _siteMapResults = siteMapExploreResults;
        }

        public IEnumerable<ContentLoadResult> GetOnlyInSiteMapResults()
        {
            return _siteMapResults.Except(_crawlerResults);
        }

        public IEnumerable<ContentLoadResult> GetOnlyInCrawlerResults()
        {
            return _crawlerResults.Except(_siteMapResults);
        }

        public IEnumerable<ContentLoadResult> GetCompositeReport()
        {
            return _crawlerResults.Union(_siteMapResults);
        }
    }
}