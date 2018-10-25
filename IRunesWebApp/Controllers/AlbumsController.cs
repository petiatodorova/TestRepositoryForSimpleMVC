namespace IRunesWebApp.Controllers
{
    using IRunesWebApp.Models;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.Net;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using IRunesWebApp.Extensions;

    public class AlbumsController : BaseController
    {
        public IHttpResponse All(IHttpRequest httpRequest)
        {
            if (!this.IsAuthenticated(httpRequest))
            {
                return new RedirectResult("/users/login");
            }

            var albums = this.Db.Albums;

            var sb = new StringBuilder();

            if (albums.Any())
            {
                foreach (var album in albums)
                {
                    sb.AppendLine($@"<div><a href=""/albums/details?albumId={album.Id}""><strong>{album.Name}</strong></a></div>");
                }

                this.ViewBag["listOfAlbums"] = sb.ToString().Trim();
            }
            else
            {
                this.ViewBag["listOfAlbums"] = "There are currently no albums.";
            }

            return this.View();
        }

        public IHttpResponse Create(IHttpRequest httpRequest)
        {
            if (!this.IsAuthenticated(httpRequest))
            {
                return new RedirectResult("/users/login");
            }

            return this.View();
        }

        public IHttpResponse DoCreate(IHttpRequest httpRequest)
        {
            var albumName = httpRequest.FormData["name"].ToString().Trim().UrlDecode();
            var cover = httpRequest.FormData["cover"].ToString().UrlDecode();

            if (this.Db.Albums.Any(a => a.Name == albumName))
            {
                return new RedirectResult("/albums/create");
            }

            var album = new Album
            {
                Id = Guid.NewGuid().ToString(),
                Name = albumName,
                Cover = cover
            };            

            this.Db.Albums.Add(album);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception)
            {
                return new RedirectResult("/albums/create");
            }

            return new RedirectResult("/albums/create");
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            var albumId = request.QueryData["albumId"].ToString();

            var album = this.Db.Albums.FirstOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                return new RedirectResult("/albums/all");
            }

            var trackList = new StringBuilder();

            trackList.AppendLine(@"<ol>");            
            foreach (var track in album.Tracks)
            {
                trackList
                    .AppendLine($@"<li><a href=""/tracks/details?albumId={albumId}&trackId={track.TrackId}""><i>{track.Track.Name}<i></a></li>");
            }
            trackList.AppendLine(@"</ol>");

            this.ViewBag["AlbumId"] = album.Id;
            this.ViewBag["Name"] = album.Name;
            this.ViewBag["Price"] = Math.Round(album.Price - (album.Price * 13 / 100), 2).ToString(CultureInfo.InvariantCulture);
            this.ViewBag["Cover"] = album.Cover;
            
            if (trackList.Length == 0)
            {
                this.ViewBag["Tracks"] = "There are currently no tracks.";
            }
            else
            {
                this.ViewBag["Tracks"] = trackList.ToString().Trim();
            }            

            return this.View();
        }
    }
}
