namespace SiteEvaluator.Html.Nodes
{
    public abstract class HtmlNode
    {
        protected abstract string Name { get; }

        public string OpenNodeTag => "<" + Name;

        public string CloseNodeTag => "</" + Name + ">";

        [HtmlNodeAttribute] 
        public string? Id { get; set; }

        [HtmlNodeAttribute]
        public string? Class { get; set; }

        [HtmlNodeAttribute]
        public string? Style { get; set; }
    }
}