namespace SIS.WebServer.Results
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;

    public class InlineResourceResult : HttpResponse
    {
        public InlineResourceResult(byte[] content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            this.Headers.Add(new HttpHeader(HttpHeader.ContentLengthHeaderKey, content.Length.ToString()));
            this.Headers.Add(new HttpHeader(HttpHeader.ContentDispositionHeaderKey, "inline"));
            this.Content = content;
        }
    }
}
