namespace SiteEvaluator.Html.Tags
{
    public abstract class HtmlTag
    {
        protected abstract string Name { get; }

        public string OpenTag => "<" + Name;

        public string CloseTag => "</" + Name + ">";

        [HtmlTagAttribute] 
        public string? Id { get; set; }

        [HtmlTagAttribute]
        public string? Class { get; set; }

        [HtmlTagAttribute]
        public string? Style { get; set; }
    }
}