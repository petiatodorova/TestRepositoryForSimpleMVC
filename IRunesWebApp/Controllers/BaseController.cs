namespace IRunesWebApp.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    using IRunesWebApp.Contracts;
    using IRunesWebApp.Data;
    using IRunesWebApp.Services;
    using SIS.Framework.Controllers;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public abstract class BaseController
    {
        private const string RootDirectoryRelativePath = "../../../";

        private const string ViewsFolderName = "Views";

        private const string ControllerDefaultName = "Controller";

        private const string HtmlFileExtension = ".html";

        private const string DirectorySeparator = "/";

        private const string LoggedOutLayoutFileName = "_LayoutLoggedOut";

        private const string LoggedInLayoutFileName = "_LayoutLoggedIn";

        private const string RenderBodyConstant = "@RenderBody()";

        private string GetCurrentControllerName() =>
            this.GetType().Name.Replace(ControllerDefaultName, string.Empty);

        protected BaseController()
        {
            this.Db = new IRunesDbContext();
            this.HashService = new HashService();
            this.UserCookieService = new UserCookieService();
            this.ViewBag = new Dictionary<string, string>();
        }

        protected IRunesDbContext Db { get; }

        protected IHashService HashService { get; }

        protected IUserCookieService UserCookieService { get; }

        protected IDictionary<string, string> ViewBag { get; set; }

        protected string GetUsername(IHttpRequest httpRequest)
        {
            if (!httpRequest.Cookies.ContainsCookie(".auth-IRunes"))
            {
                return null;
            }

            var cookieContent = httpRequest.Cookies.GetCookie(".auth-IRunes").Value;

            var username = this.UserCookieService.GetUserData(cookieContent);

            return username;
        }

        public bool IsAuthenticated(IHttpRequest httpRequest)
        {
            return httpRequest.Cookies.ContainsCookie(".auth-IRunes");
        }

        protected IHttpResponse View([CallerMemberName] string viewName = "")
        {
            var layoutPath = string.Empty;

            if (viewName == "Index" || viewName == "Login" || viewName == "Register")
            {
                layoutPath =
                    RootDirectoryRelativePath +
                    ViewsFolderName +
                    DirectorySeparator +
                    LoggedOutLayoutFileName +
                    HtmlFileExtension;
            }
            else
            {
                layoutPath =
                    RootDirectoryRelativePath +
                    ViewsFolderName +
                    DirectorySeparator +
                    LoggedInLayoutFileName +
                    HtmlFileExtension;
            }                

            var filePath =
                RootDirectoryRelativePath +
                ViewsFolderName + DirectorySeparator +
                this.GetCurrentControllerName() + DirectorySeparator +
                viewName + HtmlFileExtension;

            if (!File.Exists(filePath))
            {
                return this.NotFoundError($"View {viewName} not found.");
            }

            var layoutContent = File.ReadAllText(layoutPath);
            var renderBodyContent = GetRenderBodyContent(filePath);

            var viewContent = layoutContent.Replace(RenderBodyConstant, renderBodyContent);

            return new HtmlResult(viewContent, HttpResponseStatusCode.OK);
        }

        private string GetRenderBodyContent(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);

            foreach (var viewBagKey in this.ViewBag.Keys)
            {
                var dinamicDataPlaceholder = $"{{{{{viewBagKey}}}}}";

                if (fileContent.Contains(dinamicDataPlaceholder))
                {
                    fileContent = fileContent.Replace(dinamicDataPlaceholder, this.ViewBag[viewBagKey]);
                }
            }

            return fileContent;
        }

        protected IHttpResponse NotFoundError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.NotFound);
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.BadRequest);
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            return new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.InternalServerError);
        }
    }
}
