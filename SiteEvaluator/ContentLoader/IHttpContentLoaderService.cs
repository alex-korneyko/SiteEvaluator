using System.Threading.Tasks;

namespace SiteEvaluator.ContentLoader
{
    public interface IHttpContentLoaderService
    {
        Task<ContentLoadResult> LoadContentAsync(string pageUrl);
        Task<ContentLoadResult> LoadRobotsAsync(string hostUrl);
        Task<ContentLoadResult> LoadSiteMapAsync(string hostUrl);
    }
}