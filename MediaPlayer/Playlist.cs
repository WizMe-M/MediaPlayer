using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3.Frames;

namespace MediaPlayer
{
    [Serializable]
    public class Playlist
    {
        public string Name { get; set; }
        public List<Music> Music { get; set; }
        public List<Media> Medias { get; set; }
        Playlist()
        {
            Music = new List<Music>();
            Medias = new List<Media>();
        }
        public Playlist(string Name) : this()
        {
            this.Name = Name;
        }
    }
}
