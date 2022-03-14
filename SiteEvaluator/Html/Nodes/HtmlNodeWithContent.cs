namespace SiteEvaluator.Html.Tags
{
    public abstract class HtmlNodeWithContent : HtmlNode
    {
        public string Content { get; set; } = string.Empty;
    }
}