using System.Collections.Generic;
using SiteEvaluator.Html.Tags;

namespace SiteEvaluator.Html
{
    public interface IHtmlParseService
    {
        List<string> GetNodesAsStringsList<T>(string rawHtml) where T : HtmlNode, new();
        string ExtractBodyNode(string rawHtml);
        T? DeserializeToNode<T>(string tagFullString) where T : HtmlNode, new();
    }
}