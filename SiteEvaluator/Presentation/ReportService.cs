using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.Data;

namespace SiteEvaluator.Presentation
{
    public class ReportService : IReportService
    {
        private readonly IDao<PageInfo> _dao;

        public ReportService(IDao<PageInfo> dao)
        {
            _dao = dao;
        }

        public async Task AddCrawlerResultsAsync(
            string hostUrl,
            IEnumerable<PageInfo> crawlerResults)
        {
           await _dao.SaveCrawlerResultsDataAsync(hostUrl, crawlerResults);
        }

        public async Task AddSiteMapExplorerResultsAsync(
            string hostUrl,
            IEnumerable<PageInfo> siteMapExploreResults)
        {
            await _dao.SaveSiteMapResultsDataAsync(hostUrl, siteMapExploreResults);
        }

        public async Task<IEnumerable<PageInfo>> GetCrawlerResultsAsync(string hostUrl)
        {
            return await _dao.GetCrawlerResultsData(hostUrl);
        }

        public async Task<IEnumerable<PageInfo>> GetSiteMapResultsAsync(string hostUrl)
        {
            return await _dao.GetSiteMapResultsData(hostUrl);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqInSiteMapResults(string hostUrl)
        {
            var crawlerResultsData = await _dao.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dao.GetSiteMapResultsData(hostUrl);
            
            return siteMapResultsData.Except(crawlerResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqCrawlerResults(string hostUrl)
        {
            var crawlerResultsData = await _dao.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dao.GetSiteMapResultsData(hostUrl);
            
            return crawlerResultsData.Except(siteMapResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetCompositeReportAsync(string hostUrl)
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