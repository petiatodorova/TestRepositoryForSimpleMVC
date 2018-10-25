namespace IRunesWebApp.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using IRunesWebApp.Extensions;
    using IRunesWebApp.Models;

    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public class TracksController : BaseController
    {
        public IHttpResponse Create(IHttpRequest httpRequest)
        {
            if (!this.IsAuthenticated(httpRequest))
            {
                return new RedirectResult("/users/login");
            }

            var albumId = httpRequest.QueryData["albumId"].ToString();

            this.ViewBag["albumId"] = albumId;

            return this.View();
        }

        public IHttpResponse DoCreate(IHttpRequest httpRequest)
        {
            var albumId = httpRequest.QueryData["albumId"].ToString();

            var trackName = httpRequest.FormData["name"].ToString().Trim().UrlDecode();
            var link = httpRequest.FormData["link"].ToString().UrlDecode();
            var price = decimal.Parse(httpRequest.FormData["price"].ToString());

            if (this.Db.Tracks.Any(t => t.Name == trackName))
            {
                return new RedirectResult("/tracks/create");
            }

            var album = this.Db.Albums.FirstOrDefault(a => a.Id == albumId);

            var track = new Track
            {
                Id = Guid.NewGuid().ToString(),
                Name = trackName,
                Link = link,
                Price = price
            };

            var trackAlbum = new TrackAlbum
            {
                Track = track,
                Album = album
            };

            this.Db.TracksAlbums.Add(trackAlbum);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return new RedirectResult("/tracks/create");
            }

            return new RedirectResult($"/tracks/create?albumId={albumId}");
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            var albumId = request.QueryData["albumId"].ToString();
            var trackId = request.QueryData["trackId"].ToString();

            var albumTrack = this.Db
                .TracksAlbums
                .FirstOrDefault(x => x.TrackId == trackId && x.AlbumId == albumId);

            if (albumTrack == null)
            {
                return new RedirectResult($"/albums/details?albumId={albumId}");
            }

            this.ViewBag["AlbumId"] = albumId;
            this.ViewBag["Name"] = albumTrack.Track.Name;
            this.ViewBag["Price"] = albumTrack.Track.Price.ToString(CultureInfo.InvariantCulture);
            this.ViewBag["Link"] = albumTrack.Track.Link;

            return this.View();
        }
    }
}
