using System.Net;

namespace MockHttp
{
    internal class Response
    {
        public string ResponseContent { get; }
        public string ResponseMediaType { get; }
        public HttpStatusCode ResponseHttpStatusCode { get; }

        internal Response(
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