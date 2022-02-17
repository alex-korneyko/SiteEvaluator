using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExplorer;

public interface ISiteMapExplorer
{
    Task<SiteMapExploreResult> ExploreAsync(string hostUrl);
}