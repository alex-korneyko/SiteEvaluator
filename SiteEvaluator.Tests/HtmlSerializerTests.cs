using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class HtmlSerializerTests
    {
        [Fact]
        public void GetAllTagFullStrings_RawHtmlString_TagStringsList()
        {
            var htmlParseService = new HtmlParseService();

            var allTagFullStrings = htmlParseService.GetNodesAsStringsList<Body>(GetRawHtmlString());

            Assert.NotEmpty(allTagFullStrings);
            Assert.Collection(allTagFullStrings, tagString =>
            {
                Assert.NotNull(tagString);
                Assert.StartsWith("<body", tagString);
                Assert.EndsWith("</body>", tagString);
            });
        }

        [Fact]
        public void GetBody_RawHtmlString_ShouldReturnBodyString()
        {
            var htmlParseService = new HtmlParseService();
            
            var body = htmlParseService.ExtractBodyNode(GetRawHtmlString());

            Assert.Equal(GetBodyString(), body);
        }

        [Fact]
        public void Deserialize_BodyString_ShouldReturnBodyHtmlTag()
        {
            var htmlParseService = new HtmlParseService();
            
            var body = htmlParseService.DeserializeToNode<Body>(GetBodyString());

            Assert.NotNull(body);
            Assert.IsType<Body>(body);
            Assert.Equal("<body", body.OpenNodeTag);
            Assert.Equal("</body>", body.CloseNodeTag);
        }

        [Fact]
        public void Deserialize_AString_ShouldReturnAHtmlTag()
        {
            var htmlParseService = new HtmlParseService();
            
            var a = htmlParseService.DeserializeToNode<A>(GetAString());

            Assert.NotNull(a);
            Assert.IsType<A>(a);
            Assert.Equal("<a", a.OpenNodeTag);
            Assert.Equal("</a>", a.CloseNodeTag);
            Assert.NotNull(a.Href);
            Assert.Equal("https://i.ua", a.Href);
            Assert.NotNull(a.Rel);
            Assert.Equal("nofollow", a.Rel);
        }

        private string GetRawHtmlString()
        {
            return "" +
                   "<html>" +
                   "<head>" +
                   "</head>" +
                   GetBodyString() +
                   "</html>";
        }

        private string GetBodyString()
        {
            return "<body>" +
                   "<h1>Hello World!</h1>" +
                   GetAString() +
                   "</body>";
        }

        private string GetAString()
        {
            return "<a href=\"https://i.ua\" rel=\"nofollow\">some link</a>";
        }
    }
}