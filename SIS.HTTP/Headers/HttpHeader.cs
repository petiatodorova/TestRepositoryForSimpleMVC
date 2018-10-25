namespace SIS.HTTP.Headers
{
    using SIS.HTTP.Common;

    public class HttpHeader
    {
        public const string HostHeaderKey = "Host";

        public const string RequestCookieHeaderKey = "Cookie";

        public const string ResponseCookieHeaderKey = "Set-Cookie";

        public const string ContentTypeHeaderKey = "Content-Type";

        public const string ContentLengthHeaderKey = "Content-Length";

        public const string ContentDispositionHeaderKey = "Content-Disposition";

        public const string AuthorizationHeaderKey = "Authorization";

        public const string LocationHeaderKey = "Location";

        public HttpHeader(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{this.Key}: {this.Value}";
        }
    }
}
