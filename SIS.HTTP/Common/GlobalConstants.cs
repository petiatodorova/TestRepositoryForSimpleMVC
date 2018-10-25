namespace SIS.HTTP.Common
{
    public static class GlobalConstants
    {
        public const string HttpOneProtocolFragment = "HTTP/1.1";

        public const char HttpRequestUrlQuerySeparator = '?';

        public const char HttpRequestUrlFragmentSeparator = '#';

        public const string HttpRequestHeaderNameValueSeparator = ": ";

        public const string HttpRequestCookiesSeparator = "; ";

        public const char HttpRequestCookieNameValueSeparator = '=';

        public const char HttpRequestParameterSeparator = '&';

        public const char HttpRequestParameterNameValueSeparator = '=';

        public static string[] ResourceExtensions = { ".css", ".js" };
    }
}
