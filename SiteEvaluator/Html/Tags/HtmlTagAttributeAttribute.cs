namespace SiteEvaluator.Html.Tags;

[AttributeUsage(AttributeTargets.Property)]
public class HtmlTagAttributeAttribute : Attribute
{
    public string AttributeName { get; } = string.Empty;

    public HtmlTagAttributeAttribute()
    {
    }

    public HtmlTagAttributeAttribute(string attributeName)
    {
        AttributeName = attributeName;
    }
}