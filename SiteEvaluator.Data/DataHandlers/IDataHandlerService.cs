using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataHandlers
{
    public interface IDataHandlerService
    {
        IEnumerable<TargetHost> GetAllAsync();
        Task<TargetHost> GetTargetHostAsync(Uri hostUri);
        // Task<TargetHost> GetCrawlerResultsData(Uri hostUri);
        // Task<TargetHost> GetSiteMapResultsData(Uri hostUri);
        Task<TargetHost> SaveTargetHostAsync(TargetHost targetHost);
        // Task<TargetHost> SaveCrawlerResultsDataAsync(Uri hostUri, IEnumerable<PageInfo> data);
        // Task<TargetHost> SaveSiteMapResultsDataAsync(Uri hostUri, IEnumerable<PageInfo> data);
    }
}