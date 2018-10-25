namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Extensions;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Headers.Contracts;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Sessions.Contracts;

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {            
            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();
            this.RequestMethod = new HttpRequestMethod();            

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpCookieCollection Cookies { get; }

        public IHttpSession Session { get; set; }

        private bool IsValidRequestLine(string[] requestLine)
        {
            return requestLine.Length == 3 && requestLine[2].ToLower() != GlobalConstants.HttpOneProtocolFragment;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            return !(string.IsNullOrEmpty(queryString) || queryParameters.Length < 1);
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            var parseResult = Enum.TryParse<HttpRequestMethod>(requestLine[0].Capitalize(), true, out var parsedRequestMethod);

            if (!parseResult)
            {
                throw new BadRequestException();
            }

            this.RequestMethod = parsedRequestMethod;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            this.Path = this.Url
                .Split(new[] { GlobalConstants.HttpRequestUrlQuerySeparator,
                    GlobalConstants.HttpRequestUrlFragmentSeparator }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private void ParseHeaders(string[] requestHeaders)
        {
            foreach (var requestHeader in requestHeaders)
            {
                if (string.IsNullOrEmpty(requestHeader))
                {
                    break;
                }

                var splitRequestHeader = requestHeader.Split(GlobalConstants.HttpRequestHeaderNameValueSeparator);

                var requestHeaderKey = splitRequestHeader[0];
                var requestHeaderValue = splitRequestHeader[1];

                var httpRequestHeader = new HttpHeader(requestHeaderKey, requestHeaderValue);

                this.Headers.Add(httpRequestHeader);
            }

            if (!this.Headers.ContainsHeader(HttpHeader.HostHeaderKey))
            {
                throw new BadRequestException();
            }
        }

        private void ParseQueryParameters()
        {
            if (!this.Url.Contains(GlobalConstants.HttpRequestUrlQuerySeparator))
            {
                return;
            }

            string queryString = this.Url
                .Split(new[] { GlobalConstants.HttpRequestUrlQuerySeparator,
                    GlobalConstants.HttpRequestUrlFragmentSeparator }, StringSplitOptions.None)[1];

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return;
            }

            string[] queryParameters = queryString.Split(GlobalConstants.HttpRequestParameterSeparator);

            if (!this.IsValidRequestQueryString(queryString, queryParameters))
            {
                throw new BadRequestException();
            }

            foreach (var queryParameter in queryParameters)
            {
                string[] parameterArguments = queryParameter
                    .Split(GlobalConstants.HttpRequestParameterNameValueSeparator, StringSplitOptions.RemoveEmptyEntries);

                this.QueryData.Add(parameterArguments[0], parameterArguments[1]);
            }
        }

        private void ParseFormDataParameters(string bodyParameters)
        {
            if (string.IsNullOrEmpty(bodyParameters))
            {
                return;
            }

            var formDataKeyValueParameters = bodyParameters.Split(GlobalConstants.HttpRequestParameterSeparator);

            foreach (var kvp in formDataKeyValueParameters)
            {
                var formDataKeyValuePair = kvp
                    .Split(GlobalConstants.HttpRequestParameterNameValueSeparator, StringSplitOptions.RemoveEmptyEntries);

                if (formDataKeyValuePair.Length != 2)
                {
                    throw new BadRequestException();
                }

                var formDataKey = formDataKeyValuePair[0];
                var formDataValue = formDataKeyValuePair[1];

                this.FormData.Add(formDataKey, formDataValue);
            }
        }

        private void ParseRequestParameters(string bodyParameters)
        {
            this.ParseQueryParameters();

            this.ParseFormDataParameters(bodyParameters);
        }

        private void ParseCookies()
        {
            if (!this.Headers.ContainsHeader(HttpHeader.RequestCookieHeaderKey))
            {
                return;
            }

            var cookieRow = this.Headers.GetHeader(HttpHeader.RequestCookieHeaderKey).Value;

            var splitCookies = cookieRow.Split(GlobalConstants.HttpRequestCookiesSeparator);

            foreach (var cookie in splitCookies)
            {
                var cookieParts = cookie
                    .Split(GlobalConstants.HttpRequestCookieNameValueSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

                if (cookieParts.Length != 2)
                {
                    continue;
                }

                string key = cookieParts[0];
                string value = cookieParts[1];

                this.Cookies.Add(new HttpCookie(key, value, false));
            }
        }

        private void ParseRequest(string requestString)
        {
            var splitRequestContent = requestString
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var requestLine = splitRequestContent[0]
                .Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseHeaders(splitRequestContent.Skip(1).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(splitRequestContent[splitRequestContent.Length - 1]);
        }
    }
}
