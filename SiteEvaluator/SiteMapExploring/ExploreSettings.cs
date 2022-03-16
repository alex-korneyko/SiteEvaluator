using System;
using System.Collections.Generic;
using SiteEvaluator.DataLoader;

namespace SiteEvaluator.SiteMapExploring
{
    public class ExploreSettings
    {
        public bool LoadContent { get; set; } = false;
        public List<string> UrlsForExcludeLoadContent { get; set; } = new();
        public Action<StringLoadResult>? ExploreHtmlLoadedEvent { get; set; }
        public Action<ImageLoadResult>? ExploreImageLoadedEvent { get; set; }
    }
}