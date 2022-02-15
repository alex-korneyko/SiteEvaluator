
using SiteEvaluator;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;

var commandLineArgs = Environment.GetCommandLineArgs();

var baseUrl = "https://www.ukad-group.com/";

var pageLoader = new PageLoader();

var loadedPage = await pageLoader.LoadPage(baseUrl);

var body = HtmlSerializer.GetBody(loadedPage);

if (body != "")
{
    var allALinks = HtmlSerializer.GetAllTagStrings<A>(body);
    var aLinks = new List<A>();

    foreach (var aStringLink in allALinks)
    {
        var aLink = HtmlSerializer.Deserialize<A>(aStringLink);
        if (aLink != null)
        {
            aLinks.Add(aLink);
        }
    }

    Console.WriteLine(aLinks.Count);
}