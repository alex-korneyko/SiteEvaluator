using System;
using System.Collections.Generic;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.Common
{
    public static class Utils
    {
        public static List<A> FilterOuterLinksNodes(List<A> aNodes, Uri hostUri)
        {
            var result = new List<A>();

            foreach (var aNode in aNodes)
            {
                if (string.IsNullOrEmpty(aNode.Href) || aNode.Href.StartsWith('#') || aNode.Href.StartsWith('/'))
                    continue;

                var aNodeUri = new Uri(aNode.Href);

                if (!aNodeUri.Host.Equals(hostUri.Host))
                    result.Add(aNode);
            }

            return result;
        }

        public static List<A> FilterInnerLinkNodes(List<A> aNodes, Uri hostUri)
        {
            var result = new List<A>();
            
            foreach (var aNode in aNodes)
            {
                if (aNode.Href == null || aNode.Href.StartsWith('#'))
                    continue;

                if (aNode.Href.StartsWith('/') || aNode.Href.StartsWith("..") || aNode.Href.Equals(""))
                {
                    result.Add(aNode);
                    continue;
                }

                if (new Uri(aNode.Href).Host.Equals(hostUri.Host))
                    result.Add(aNode);
            }

            return result;
        }
    }
}