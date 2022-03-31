using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataHandlers
{
    public interface IDataHandlerService<T> where T : IEntity
    {
        Task<IEnumerable<T>> GetCrawlerResultsData(string hostUrl);
        Task<IEnumerable<T>> GetSiteMapResultsData(string hostUrl);
        Task<long> SaveCrawlerResultsDataAsync(string hostUrl, IEnumerable<T> data);
        Task<long> SaveSiteMapResultsDataAsync(string hostUrl, IEnumerable<T> data);
    }
}