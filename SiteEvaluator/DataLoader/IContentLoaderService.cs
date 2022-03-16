using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader
{
    public interface IContentLoaderService
    {
        // Task<ContentLoadResult> LoadContentAsync(string pageUrl);
        Task<StringLoadResult> LoadRobotsAsync(string hostUrl);
        Task<StringLoadResult> LoadSiteMapAsync(string hostUrl);
        Task<StringLoadResult> LoadHtmlAsync(string requestUri);
        Task<ImageLoadResult> LoadImageAsync(string requestUri);
        Task<FileLoadResult> LoadFile(string requestUri);
    }
}