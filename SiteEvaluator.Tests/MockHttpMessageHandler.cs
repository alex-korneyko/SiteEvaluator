using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteEvaluator.Tests
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

    public class MockHttpMessageHandlerSetupResult
    {
        private readonly string _targetUri;
        private readonly Dictionary<string, Response> _requestResponseSet;

        public MockHttpMessageHandlerSetupResult(string targetUri, Dictionary<string, Response> requestResponseSet)
        {
            _targetUri = targetUri;
            _requestResponseSet = requestResponseSet;
        }

        public void Returns(
            string responseContent, 
            string responseMediaType = "text/html", 
            HttpStatusCode responseHttpStatusCode = HttpStatusCode.OK)
        {
            var response = new Response(responseContent, responseMediaType, responseHttpStatusCode);

            _requestResponseSet.Remove(_targetUri);
            _requestResponseSet.Add(_targetUri, response);
        }
    }

    public class Response
    {
        public string ResponseContent { get; }
        public string ResponseMediaType { get; }
        public HttpStatusCode ResponseHttpStatusCode { get; }

        public Response(
            string responseContent = "",
            string responseMediaType = "text/html",
            HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            ResponseContent = responseContent;
            ResponseMediaType = responseMediaType;
            ResponseHttpStatusCode = httpStatusCode;
        }
    }
}