using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExplorer;

public class SiteMapExplorer : ISiteMapExplorer
{
    private readonly IHttpContentLoader _httpContentLoader;
    private readonly SiteMapExplorerSettings _settings = new();

    public SiteMapExplorer(IHttpContentLoader httpContentLoader)
    {
        _httpContentLoader = httpContentLoader;
    }

    public SiteMapExplorer(IHttpContentLoader httpContentLoader, Action<SiteMapExplorerSettings> settings) : this(httpContentLoader)
    {
       settings.Invoke(_settings);
    }

    public async Task<SiteMap> ExploreAsync(string hostUrl)
    {
        ConsoleMessage.WriteLineWarning("\nStart sitemap.xml exploring...");
        
        var loadSiteMapResult = await _httpContentLoader.LoadSiteMapAsync(hostUrl);

        if (!loadSiteMapResult.IsSuccess) return new SiteMap();
        
        var siteMapString = loadSiteMapResult.Content;
        var siteMap = SiteMapSerializer.Deserialize(siteMapString);

        ConsoleMessage.WriteLineSuccess("sitemap.xml exploring finished!");
        if (_settings.PrintResult) PrintToConsole(siteMap);

        return siteMap;
    }

    private static void PrintToConsole(SiteMap siteMap)
    {
        if (siteMap.UrlSet == null) return;

        ConsoleMessage.WriteLineSuccess("sitemap.xml exploring result: ");
        foreach (var url in siteMap.UrlSet)
        {
            Console.WriteLine($"loc: {url.Loc}\tlastmod: {url.LastMod}");
        }
    }
}