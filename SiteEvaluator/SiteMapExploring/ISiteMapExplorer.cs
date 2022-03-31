using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.SiteMapExploring
{
    public interface ISiteMapExplorer
    {
        Task<IList<PageInfo>> ExploreAsync(string hostUrl, Action<ExploreSettings>? exploreSettings);
    }
}