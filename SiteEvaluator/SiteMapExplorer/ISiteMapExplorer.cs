using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExplorer;

public interface ISiteMapExplorer
{
    Task<SiteMap> ExploreAsync(string hostUrl);
}