using System.Collections.Generic;

namespace SiteEvaluator.SiteMapExploring
{
    public class ExploreSettings
    {
        public bool LoadContent { get; set; } = false;
        public List<string> UrlsForExcludeLoadContent { get; set; } = new();
        public bool PrintResult { get; set; }
    }
}