namespace SiteEvaluator.Html.Tags;

public abstract class HtmlTagWithContent : HtmlTag
{
    public string Content { get; set; } = string.Empty;
}