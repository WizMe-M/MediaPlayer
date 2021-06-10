using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3.Frames;

namespace MediaPlayer
{
    class Playlist
    {
        public string Name { get; set; }
        public List<Music> Music { get; set; }
        Playlist()
        {
            Music = new List<Music>();
        }
        public Playlist(string Name) : this()
        {
            this.Name = Name;
        }
    }
}
