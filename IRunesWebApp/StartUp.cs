namespace IRunesWebApp
{
    using IRunesWebApp.Controllers;
    using SIS.Framework;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Results;
    using SIS.WebServer.Routing;

    public class StartUp
    {
        public static void Main()
        {
            var serverRoutingTable = new ServerRoutingTable();
            //GET
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/home/index"] = request => new RedirectResult("/");
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/users/login"] = request => new UsersController().Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/users/logout"] = request => new UsersController().Logout(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/users/register"] = request => new UsersController().Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/albums/all"] = request => new AlbumsController().All(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/albums/create"] = request => new AlbumsController().Create(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/albums/details"] = request => new AlbumsController().Details(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/tracks/create"] = request => new TracksController().Create(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/tracks/details"] = request => new TracksController().Details(request);

            //POST
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/users/login"] = request => new UsersController().DoLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/users/register"] = request => new UsersController().DoRegister(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/albums/create"] = request => new AlbumsController().DoCreate(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/tracks/create"] = request => new TracksController().DoCreate(request);

            var handler = new HttpHandler(serverRoutingTable);

            var server = new Server(8000, handler);

            server.Run();

        }
    }
}
