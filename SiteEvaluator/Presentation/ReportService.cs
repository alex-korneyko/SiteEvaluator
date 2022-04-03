using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Presentation
{
    public class ReportService : IReportService
    {
        private readonly IDataHandlerService _dataHandlerService;

        public ReportService(IDataHandlerService dataHandlerService)
        {
            _dataHandlerService = dataHandlerService;
        }

        // public async Task AddCrawlerResultsAsync(
        //     Uri hostUri,
        //     IEnumerable<PageInfo> crawlerResults)
        // {
        //    await _dataHandlerService.SaveTargetHost(hostUri, crawlerResults);
        // }
        //
        // public async Task AddSiteMapExplorerResultsAsync(
        //     Uri hostUri,
        //     IEnumerable<PageInfo> siteMapExploreResults)
        // {
        //     await _dataHandlerService.SaveTargetHost(hostUri, siteMapExploreResults);
        // }

        public async Task<IEnumerable<PageInfo>> GetCrawlerResultsAsync(Uri hostUri)
        {
            return (await _dataHandlerService.GetTargetHostAsync(hostUri)).PageInfos
                .Where(page => page.ScannerType == ScannerType.SiteCrawler);
        }

        public async Task<IEnumerable<PageInfo>> GetSiteMapResultsAsync(Uri hostUri)
        {
            return (await _dataHandlerService.GetTargetHostAsync(hostUri)).PageInfos
                .Where(page => page.ScannerType == ScannerType.SiteCrawler);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqInSiteMapResults(Uri hostUri)
        {
            var crawlerResultsData = await GetCrawlerResultsAsync(hostUri);
            var siteMapResultsData = await GetSiteMapResultsAsync(hostUri);
            
            return siteMapResultsData.Except(crawlerResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetUniqCrawlerResults(Uri hostUri)
        {
            var crawlerResultsData = await GetCrawlerResultsAsync(hostUri);
            var siteMapResultsData = await GetSiteMapResultsAsync(hostUri);
            
            return crawlerResultsData.Except(siteMapResultsData);
        }

        public async Task<IEnumerable<PageInfo>> GetCompositeReportAsync(Uri hostUri)
        {
            var crawlerResultsData = await GetCrawlerResultsAsync(hostUri);
            var siteMapResultsData = await GetSiteMapResultsAsync(hostUri);

            var compositeReport = crawlerResultsData
                .Union(siteMapResultsData)
                .ToList();
            
            compositeReport.Sort();
            
            return compositeReport;
        }
    }
}