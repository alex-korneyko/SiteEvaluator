namespace SiteEvaluator.Html.Tags;

public abstract class HtmlTag
{
    protected abstract string Name { get; }

    public string OpenTag => "<" + Name;
    
    public string CloseTag => "</" + Name + ">";

}