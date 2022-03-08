using System.Collections.Generic;
using System.Net;

namespace MockHttp
{
    public class MockHttpMessageHandlerSetupResult
    {
        private readonly string _targetUri;
        private readonly Dictionary<string, Response> _requestResponseSet;

        internal MockHttpMessageHandlerSetupResult(string targetUri, Dictionary<string, Response> requestResponseSet)
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
}