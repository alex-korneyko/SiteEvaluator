namespace SiteEvaluator.PageLoader;

public interface IHttpContentLoader
{
    Task<ContentLoadResult> LoadContentAsync(string pageUrl);
    Task<ContentLoadResult> LoadRobotsAsync(string hostUrl);
    Task<ContentLoadResult> LoadSiteMapAsync(string hostUrl);
}