namespace SiteEvaluator.PageLoader;

public interface IPageLoader
{
    Task<PageLoadResult> LoadPageAsync(string pageUrl);
}