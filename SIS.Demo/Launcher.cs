namespace SIS.Demo
{
    using System;
    using SIS.Framework;
    using SIS.Framework.Routers;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Launcher
    {
        public static void Main()
        {
            var handler = new ControllerRouter();

            Server server = new Server(8000, new ControllerRouter());

            MvcEngine.Run(server);
        }
    }
}
