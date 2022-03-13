namespace SiteEvaluator.Xml
{
    public interface ISiteMapParseService
    {
        SiteMap DeserializeToSiteMap(string xmlString);
    }
}