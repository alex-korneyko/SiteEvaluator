using System;
using System.Collections.Generic;

namespace SiteEvaluator.Data.Model
{
    public class TargetHost : IEntity
    {
        public int Id { get; set; }
        public string HostUrl { get; set; }
        public List<PageInfo> PageInfos { get; set; } = new();
        public DateTime EvaluationDate { get; set; } = DateTime.Now;
        public bool RobotsIsMissing { get; set; }
        public bool SiteMapError { get; set; }

        public TargetHost()
        {
        }

        public TargetHost(Uri hostUri)
        {
            HostUrl = hostUri.Host;
        }
    }
}