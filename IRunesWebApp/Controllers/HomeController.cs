namespace IRunesWebApp.Controllers
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;

    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest httpRequest)
        {
            if (this.IsAuthenticated(httpRequest))
            {
                var username = this.GetUsername(httpRequest);
                this.ViewBag["username"] = username;

                return this.View("LoggedIn");
            }

            return this.View();
        }
    }
}
