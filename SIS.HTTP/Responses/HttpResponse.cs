namespace SIS.HTTP.Responses
{
    using System.Text;
    using System.Linq;

    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.HTTP.Extensions;
    
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Cookies;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
        }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {            
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.Content = new byte[0];
            this.StatusCode = statusCode;            
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; private set; }

        public IHttpCookieCollection Cookies { get; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));

            this.Headers.Add(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.Cookies.Add(cookie);
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString()).Concat(this.Content).ToArray();
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result
                .AppendLine($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetResponseLine()}")
                .AppendLine($"{this.Headers}");

            if (this.Cookies.HasCookies())
            {
                foreach (var httpCookie in this.Cookies)
                {
                    result.AppendLine($"{HttpHeader.ResponseCookieHeaderKey}: {httpCookie}");
                }                
            }

            result.AppendLine();

            return result.ToString();
        }
    }
}
