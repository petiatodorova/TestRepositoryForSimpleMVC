namespace IRunesWebApp.Models
{
    public class TrackAlbum
    {
        public string TrackId { get; set; }
        public virtual Track Track { get; set; }

        public string AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}
