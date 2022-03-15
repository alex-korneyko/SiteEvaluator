using System;
using System.Collections.Generic;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.SiteMapExploring
{
    public class ExploreSettings
    {
        public bool LoadContent { get; set; } = false;
        public List<string> UrlsForExcludeLoadContent { get; set; } = new();
        public Action<ContentLoadResult>? ExploreEvent { get; set; }
    }
}