namespace SIS.Framework.Controllers
{
    using System.Runtime.CompilerServices;

    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.ActionsResults;
    using SIS.Framework.Models;
    using SIS.Framework.Utilities;
    using SIS.Framework.Views;
    using SIS.HTTP.Requests.Contracts;

    public abstract class Controller
    {
        protected Controller()
        {
            this.ViewModel = new ViewModel();
        }

        public IHttpRequest Request { get; set; }

        public ViewModel ViewModel { get; set; }

        public Model ModelState { get; } = new Model();

        protected IViewable View([CallerMemberName] string viewName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var viewFullyQualifiedName = ControllerUtilities
                .GetViewFullyQualifiedName(controllerName, viewName);

            var view = new View(viewFullyQualifiedName);

            return new ViewResult(view);
        }

        protected IRedirectable RedirectToAction(string redirectUrl)
            => new RedirectResult(redirectUrl);
    }
}
