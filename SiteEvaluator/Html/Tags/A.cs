namespace SiteEvaluator.Html.Tags
{ 
    /**
     * Class that represents HTML 'a' tag
     */
    public class A : HtmlTagWithContent
    {
        protected override string Name => "a";

        [HtmlTagAttribute] public string? Href { get; set; }

        [HtmlTagAttribute] public string? Rel { get; set; }
    }
}