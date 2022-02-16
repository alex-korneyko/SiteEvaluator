using System.Diagnostics;

namespace SiteEvaluator.PageLoader;

public class PageLoader : IPageLoader
{
    public async Task<PageLoadResult> LoadPageAsync(string pageUrl)
    {
        var stopwatch = new Stopwatch();
        var httpClient = new HttpClient();
        var pageLoadResult = new PageLoadResult(pageUrl);

        try
        {
            stopwatch.Start();
            var httpResponseMessage = await httpClient.GetAsync(pageUrl);
            stopwatch.Stop();

            pageLoadResult.HttpStatusCode = httpResponseMessage.StatusCode;
            pageLoadResult.PageLoadTime = stopwatch.ElapsedMilliseconds;

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return pageLoadResult;
            }

            pageLoadResult.PageContent = await httpResponseMessage.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            pageLoadResult.IsSuccess = false;
            pageLoadResult.Exception = e;

            return pageLoadResult;
        }
        
        return pageLoadResult;
    }
}