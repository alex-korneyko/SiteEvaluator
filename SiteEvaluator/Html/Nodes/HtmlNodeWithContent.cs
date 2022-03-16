namespace SiteEvaluator.Html.Nodes
{
    public abstract class HtmlNodeWithContent : HtmlNode
    {
        public string Content { get; set; } = string.Empty;
    }
}