using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteEvaluator.Tests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly MockHttpMessageHandlerSettings _settings = new();
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponseMessage = new HttpResponseMessage(_settings.TargetUri == request.RequestUri?.AbsoluteUri ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            httpResponseMessage.Content = new StringContent(_settings.ResponseContent, Encoding.UTF8, _settings.ResponseMediaType);
            return Task.FromResult(httpResponseMessage);
        }
        
        public MockHttpMessageHandlerSetupResult Setup(string targetUri)
        {
            _settings.TargetUri = targetUri;

            return new MockHttpMessageHandlerSetupResult(_settings);
        }
    }

    public class MockHttpMessageHandlerSettings
    {
        private string _targetUri;
        
        public string TargetUri
        {
            get => _targetUri;
            set => _targetUri = value.EndsWith('/') ? value : value + "/";
        }

        public string ResponseMediaType { get; set; } = "text/html";
        public string ResponseContent { get; set; }
    }

    public class MockHttpMessageHandlerSetupResult
    {
        private readonly MockHttpMessageHandlerSettings _settings;

        public MockHttpMessageHandlerSetupResult(MockHttpMessageHandlerSettings settings)
        {
            _settings = settings;
        }

        public void Returns(string responseContent, string responseMediaType)
        {
            _settings.ResponseContent = responseContent;
            _settings.ResponseMediaType = responseMediaType;
        }
    }
}