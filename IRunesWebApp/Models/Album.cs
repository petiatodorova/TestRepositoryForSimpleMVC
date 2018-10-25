namespace IRunesWebApp.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Album : BaseModel<string>
    {
        public Album()
        {
            this.Tracks = new HashSet<TrackAlbum>();
        }

        public string Name { get; set; }

        public string Cover { get; set; } //a link to an image

        public decimal Price => Tracks.Sum(t => t.Track.Price);

        public virtual ICollection<TrackAlbum> Tracks { get; set; }
    }
}
