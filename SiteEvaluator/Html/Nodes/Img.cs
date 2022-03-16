namespace SiteEvaluator.Html.Nodes
{
    public class Img : HtmlNode
    {
        protected override string Name => "img";
        
        [HtmlNodeAttribute]
        public string? Src { get; set; }
    }
}