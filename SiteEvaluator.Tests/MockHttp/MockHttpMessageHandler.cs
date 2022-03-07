using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteEvaluator.Tests.MockHttp
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, Response> _requestResponseSet = new();
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            
            if (string.IsNullOrEmpty(request.RequestUri?.OriginalString))
            {
                return Task.FromResult(httpResponseMessage);
            }

            var response = _requestResponseSet.GetValueOrDefault(request.RequestUri.OriginalString);

            if (response == null)
            {
                httpResponseMessage.StatusCode = HttpStatusCode.NotFound;
                return Task.FromResult(httpResponseMessage);
            }

            httpResponseMessage.StatusCode = response.ResponseHttpStatusCode;
            httpResponseMessage.Content = new StringContent(response.ResponseContent, Encoding.UTF8, response.ResponseMediaType);
            
            return Task.FromResult(httpResponseMessage);
        }
        
        public MockHttpMessageHandlerSetupResult Setup(string targetUri)
        {
            _requestResponseSet.Add(targetUri, new Response());

            return new MockHttpMessageHandlerSetupResult(targetUri, _requestResponseSet);
        }
    }
}