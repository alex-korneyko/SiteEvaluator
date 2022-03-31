using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.Data;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Presentation
{
    public class ReportService : IReportService
    {
        private readonly IDataHandlerService<PageInfo> _dataHandlerService;

        public ReportService(IDataHandlerService<PageInfo> dataHandlerService)
        {
            _dataHandlerService = dataHandlerService;
        }

        public async Task AddCrawlerResultsAsync(
            string hostUrl,
            IEnumerable<PageInfo> crawlerResults)
        {
           await _dataHandlerService.SaveCrawlerResultsDataAsync(hostUrl, crawlerResults);
        }

        public async Task AddSiteMapExplorerResultsAsync(
            string hostUrl,
            IEnumerable<PageInfo> siteMapExploreResults)
        {
            await _dataHandlerService.SaveSiteMapResultsDataAsync(hostUrl, siteMapExploreResults);
        }

        public async Task<IEnumerable<PageInfo>> GetCrawlerResultsAsync(string hostUrl)
        {
            return await _dataHandlerService.GetCrawlerResultsData(hostUrl);
        }

        public async Task<IEnumerable<PageInfo>> GetSiteMapResultsAsync(string hostUrl)
        {
            return await _dataHandlerService.GetSiteMapResultsData(hostUrl);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqInSiteMapResults(string hostUrl)
        {
            var crawlerResultsData = await _dataHandlerService.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dataHandlerService.GetSiteMapResultsData(hostUrl);
            
            return siteMapResultsData.Except(crawlerResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqCrawlerResults(string hostUrl)
        {
            var crawlerResultsData = await _dataHandlerService.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dataHandlerService.GetSiteMapResultsData(hostUrl);
            
            return crawlerResultsData.Except(siteMapResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetCompositeReportAsync(string hostUrl)
        {
            var crawlerResultsData = await _dataHandlerService.GetCrawlerResultsData(hostUrl);
            var siteMapResultsData = await _dataHandlerService.GetSiteMapResultsData(hostUrl);

            var compositeReport = crawlerResultsData
                .Union(siteMapResultsData)
                .ToList();
            
            compositeReport.Sort();
            
            return compositeReport;
        }
    }
}