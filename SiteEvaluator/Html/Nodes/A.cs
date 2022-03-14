namespace SiteEvaluator.Html.Tags
{ 
    /**
     * Class that represents HTML 'a' tag
     */
    public class A : HtmlNodeWithContent
    {
        protected override string Name => "a";

        [HtmlNodeAttribute] public string? Href { get; set; }

        [HtmlNodeAttribute] public string? Rel { get; set; }
    }
}