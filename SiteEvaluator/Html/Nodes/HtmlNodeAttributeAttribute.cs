using System;

namespace SiteEvaluator.Html.Tags
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HtmlNodeAttributeAttribute : Attribute
    {
        public string AttributeName { get; } = string.Empty;

        public HtmlNodeAttributeAttribute()
        {
        }

        public HtmlNodeAttributeAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }
    }
}