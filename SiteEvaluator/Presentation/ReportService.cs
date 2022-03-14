using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Data;

namespace SiteEvaluator.Presentation
{
    public class ReportService : IReportService
    {
        private readonly IDao<ContentLoadResult> _dao;

        public ReportService(IDao<ContentLoadResult> dao)
        {
            _dao = dao;
        }

        public async Task AddCrawlerResultsAsync(
            string hostUrl,
            IEnumerable<ContentLoadResult> crawlerResults)
        {
           await _dao.SaveCrawlerResultsDataAsync(hostUrl, crawlerResults);
        }

        public async Task AddSiteMapExplorerResultsAsync(
            string hostUrl,
            IEnumerable<ContentLoadResult> siteMapExploreResults)
        {
            await _dao.SaveSiteMapResultsDataAsync(hostUrl, siteMapExploreResults);
        }

        public async Task<IEnumerable<ContentLoadResult>> GetCrawlerResultsAsync(string hostUrl)
        {
            return await _dao.GetCrawlerResultsData(hostUrl);
        }

        public async Task<IEnumerable<ContentLoadResult>> GetSiteMapResultsAsync(string hostUrl)
        {
            return await _dao.GetSiteMapResultsData(hostUrl);
        }

        public async Task<IEnumerable<ContentLoadResult>> GetUniqInSiteMapResults(string hostUrl)
        {
            var crawlerResultsData = await _dao.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dao.GetSiteMapResultsData(hostUrl);
            
            return siteMapResultsData.Except(crawlerResultsData);
        }

        public async Task<IEnumerable<ContentLoadResult>> GetUniqCrawlerResults(string hostUrl)
        {
            var crawlerResultsData = await _dao.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dao.GetSiteMapResultsData(hostUrl);
            
            return crawlerResultsData.Except(siteMapResultsData);
        }

        public async Task<IEnumerable<ContentLoadResult>> GetCompositeReportAsync(string hostUrl)
        {
            var crawlerResultsData = await _dao.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dao.GetSiteMapResultsData(hostUrl);

            var compositeReport = crawlerResultsData
                .Union(siteMapResultsData)
                .ToList();
            
            compositeReport.Sort();
            
            return compositeReport;
        }
    }
}