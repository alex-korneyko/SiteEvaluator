using System;

namespace SiteEvaluator.Html.Nodes
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