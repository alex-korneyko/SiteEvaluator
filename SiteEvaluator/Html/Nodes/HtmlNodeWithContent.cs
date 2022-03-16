namespace SiteEvaluator.Html.Nodes
{
    public abstract class HtmlNodeWithContent : HtmlNode
    {
        public override string CloseNodeTag => "</" + Name + ">";
        
        public string Content { get; set; } = string.Empty;
    }
}