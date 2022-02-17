namespace SiteEvaluator.PageLoader;

public interface IHttpContentLoader
{
    Task<PageLoadResult> LoadContentAsync(string pageUrl);
    Task<PageLoadResult> LoadRobotsAsync(string hostUrl);
    Task<PageLoadResult> LoadSiteMapAsync(string hostUrl);
}