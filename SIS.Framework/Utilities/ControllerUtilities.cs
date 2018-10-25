namespace SIS.Framework.Utilities
{
    public class ControllerUtilities
    {
        public static string GetControllerName(object controller) =>
            controller
            .GetType()
            .Name
            .Replace(MvcContext.Get.ControllerSuffix, string.Empty);

        public static string GetViewFullyQualifiedName(string controllerName, string viewName) =>
            string
            .Format("../../../{0}/{1}/{2}.html",
                MvcContext.Get.ViewsFolder,
                controllerName,
                viewName);
    }
}
