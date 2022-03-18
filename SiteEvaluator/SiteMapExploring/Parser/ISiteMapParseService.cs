namespace SiteEvaluator.SiteMapExploring.Parser
{
    public interface ISiteMapParseService
    {
        SiteMap DeserializeToSiteMap(string xmlString);
    }
}