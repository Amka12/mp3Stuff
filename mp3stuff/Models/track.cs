using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Stuff
{
    class Track
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        public Track(string path, string artist, string title, string album)
        {
            Path = path;
            Artist = artist;
            Title = title;
            Album = album;
        }
    }
}
