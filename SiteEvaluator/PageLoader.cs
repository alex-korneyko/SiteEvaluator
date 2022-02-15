using System.Net;

namespace SiteEvaluator;

public class PageLoader
{
    public async Task<string> LoadPage(string pageUrl)
    {
        var httpClient = new HttpClient();
        var httpResponseMessage = await httpClient.GetAsync(pageUrl);

        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine(httpResponseMessage.StatusCode);
            return "";
        }

        var body = await httpResponseMessage.Content.ReadAsStringAsync();
        return body;

    }
}