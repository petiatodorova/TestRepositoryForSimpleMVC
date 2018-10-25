namespace SIS.Framework
{
    using System.IO;
    using System.Linq;

    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Api.Contracts;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;    

    public class HttpHandler : IHttpHandler
    {
        private const string RootDirectoryRelativePath = "../../../";
        private readonly ServerRoutingTable serverRoutingTable;

        public HttpHandler(ServerRoutingTable routingTable)
        {
            this.serverRoutingTable = routingTable;
        }

        public IHttpResponse Handle(IHttpRequest httpRequest)
        {
            

            if (!this.serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod) ||
                !this.serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            return this.serverRoutingTable.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest);
        }

        private IHttpResponse ReturnIfResource(string httpRequestPath)
        {
            var startIndexOfNameOfResource = httpRequestPath.LastIndexOf('/');
            var resourceName = httpRequestPath.Substring(startIndexOfNameOfResource);

            var requestPathExtension = httpRequestPath.Substring(httpRequestPath.LastIndexOf('.'));

            var resourcesPath =
                RootDirectoryRelativePath +
                "Resources" +
                $"/{requestPathExtension.Substring(1)}" +
                resourceName;

            if (!File.Exists(resourcesPath))
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            var fileContent = File.ReadAllBytes(resourcesPath);

            return new InlineResourceResult(fileContent, HttpResponseStatusCode.OK);
        }

        
    }
}
