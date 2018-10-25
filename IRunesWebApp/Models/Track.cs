using System.Collections.Generic;

namespace IRunesWebApp.Models
{
    public class Track : BaseModel<string>
    {
        public Track()
        {
            this.Albums = new HashSet<TrackAlbum>();
        }

        public string Name { get; set; }

        public string Link { get; set; } //a link to a video

        public decimal Price { get; set; }

        public virtual ICollection<TrackAlbum> Albums { get; set; }
    }
}
