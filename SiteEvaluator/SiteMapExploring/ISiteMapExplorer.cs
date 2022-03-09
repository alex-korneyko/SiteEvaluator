using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.SiteMapExploring
{
    public interface ISiteMapExplorer
    {
        Task<IList<ContentLoadResult>> ExploreAsync(string hostUrl, bool loadPagesContent);
    }
}