using System.Collections.Generic;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.Html
{
    public interface IHtmlParseService
    {
        List<string> GetNodesAsStringsList<T>(string rawHtml) where T : HtmlNode, new();
        List<T> GetAllNodes<T>(string rawHtml) where T : HtmlNode, new();
        string ExtractBodyNode(string rawHtml);
        T? DeserializeToNode<T>(string nodeFullString) where T : HtmlNode, new();
    }
}