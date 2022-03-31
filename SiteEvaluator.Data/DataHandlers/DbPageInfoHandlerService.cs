using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataHandlers
{
    public class DbPageInfoHandlerService : IDataHandlerService<PageInfo>
    {
        private readonly IRepository<PageInfo> _repository;

        public DbPageInfoHandlerService(IRepository<PageInfo> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PageInfo>> GetCrawlerResultsData(string hostUrl)
        {
            return await _repository
                .GetAll()
                .Where(pageInfo =>
                    pageInfo.SourceHost.Equals(hostUrl) && pageInfo.ScannerType == ScannerType.SiteCrawler)
                .Include(pageInfo => pageInfo.PageInfoUrls)
                .ToListAsync();
        }

        public async Task<IEnumerable<PageInfo>> GetSiteMapResultsData(string hostUrl)
        {
            return await _repository
                .GetAll()
                .Where(pageInfo =>
                    pageInfo.SourceHost.Equals(hostUrl) && pageInfo.ScannerType == ScannerType.SiteMap)
                .Include(pageInfo => pageInfo.PageInfoUrls)
                .ToListAsync();
        }

        public async Task<long> SaveCrawlerResultsDataAsync(string hostUrl, IEnumerable<PageInfo> data)
        {
            var crawlerResultsData = (await GetCrawlerResultsData(hostUrl)).ToList();
            crawlerResultsData.ForEach(_repository.Delete);
            await _repository.SaveChangesAsync();

            var pageInfoList = data.ToList();
            pageInfoList.ForEach(pageInfo =>
            {
                pageInfo.SourceHost = hostUrl;
                pageInfo.ScannerType = ScannerType.SiteCrawler;
                pageInfo.Content = string.Empty;
            });
            
            _repository.AddRange(pageInfoList);

            await _repository.SaveChangesAsync();

            return pageInfoList.Count;
        }

        public async Task<long> SaveSiteMapResultsDataAsync(string hostUrl, IEnumerable<PageInfo> data)
        {
            var siteMapResultsData = (await GetSiteMapResultsData(hostUrl)).ToList();
            siteMapResultsData.ForEach(_repository.Delete);
            await _repository.SaveChangesAsync();
            
            var pageInfoList = data.ToList();
            pageInfoList.ForEach(pageInfo =>
            {
                pageInfo.SourceHost = hostUrl;
                pageInfo.ScannerType = ScannerType.SiteMap;
                pageInfo.Content = string.Empty;
            });
            
            _repository.AddRange(pageInfoList);

            await _repository.SaveChangesAsync();

            return pageInfoList.Count;
        }
    }
}