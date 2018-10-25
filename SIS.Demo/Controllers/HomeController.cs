namespace SIS.Demo.Controllers
{
    using SIS.Framework.ActionResults.Base;
    using SIS.Framework.Controllers;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
