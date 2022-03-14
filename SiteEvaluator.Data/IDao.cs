using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiteEvaluator.Data
{
    public interface IDao<T>
    {
        Task<IEnumerable<T>> GetCrawlerResultsData(string hostUrl);
        Task<IEnumerable<T>> GetSiteMapResultsData(string hostUrl);
        Task<long> SaveCrawlerResultsDataAsync(string hostUrl, IEnumerable<T> data);
        Task<long> SaveSiteMapResultsDataAsync(string hostUrl, IEnumerable<T> data);
    }
}