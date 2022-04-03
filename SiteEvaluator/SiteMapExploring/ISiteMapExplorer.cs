using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.SiteMapExploring
{
    public interface ISiteMapExplorer
    {
        Task<TargetHost> ExploreAsync(Uri hostUri, Action<ExploreSettings>? exploreSettings);
    }
}