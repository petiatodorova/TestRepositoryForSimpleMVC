namespace SIS.WebServer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.HTTP.Sessions;
    using SIS.WebServer.Api.Contracts;
    using SIS.WebServer.Results;

    public class ConnectionHandler
    {
        private const string RootDirectoryRelativePath = "../../../";

        private readonly Socket client;

        private readonly IHttpHandler handler;

        public ConnectionHandler(Socket client, IHttpHandler handler)
        {
            CoreValidator.ThrowIfNull(client, nameof(client));
            CoreValidator.ThrowIfNull(handler, nameof(handler));

            this.client = client;
            this.handler = handler;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                var numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);

                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                httpResponse
                    .AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey, $"{sessionId}; HttpOnly"));
            }
        }

        private async Task PrepareResponse(IHttpResponse httpResponse)
        {
            var byteSegments = httpResponse.GetBytes();

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        public async Task ProcessRequestAsync()
        {
            try
            {
                var httpRequest = await this.ReadRequest();

                if (httpRequest != null)
                {
                    string sessionId = this.SetRequestSession(httpRequest);

                    var httpResponse = this.handler.Handle(httpRequest);

                    this.SetResponseSession(httpResponse, sessionId);

                    await this.PrepareResponse(httpResponse);
                }
            }
            catch (BadRequestException e)
            {
                await this.PrepareResponse(new TextResult(e.Message, HttpResponseStatusCode.BadRequest));
            }
            catch (Exception e)
            {
                await this.PrepareResponse(new TextResult(e.Message, HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private bool IsResourceRequest(IHttpRequest httpRequestPath)
        {
            var requestPath = httpRequestPath.Path;
            if (requestPath.Contains('.'))
            {
                var requestPathExtension = requestPath.Substring(requestPath.LastIndexOf('.'));

                return GlobalConstants.ResourceExtensions.Contains(requestPathExtension);
            }

            return false;
        }

    }
}
