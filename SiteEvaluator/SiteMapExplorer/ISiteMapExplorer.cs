using System.Threading.Tasks;

namespace SiteEvaluator.SiteMapExplorer
{
    public interface ISiteMapExplorer
    {
        Task<SiteMapExploreResult> ExploreAsync(string hostUrl);
    }
}