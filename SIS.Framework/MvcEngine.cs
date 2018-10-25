namespace SIS.Framework
{
    using System;
    using System.Reflection;

    using SIS.WebServer;

    public class MvcEngine
    {
        public static void Run(Server server)
        {
            RegisterAssemblyName();
            RegisterControllersData();
            RegisterViews();
            RegisterModels();

            try
            {
                server.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RegisterModels()
        {
            MvcContext.Get.ModelsFolder = "Models";
        }

        private static void RegisterViews()
        {
            MvcContext.Get.ViewsFolder = "Views";
        }

        private static void RegisterControllersData()
        {
            MvcContext.Get.ControllersFolder = "Controllers";
            MvcContext.Get.ControllerSuffix = "Controller";
        }

        private static void RegisterAssemblyName()
        {
            MvcContext.Get.AssemblyName = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
