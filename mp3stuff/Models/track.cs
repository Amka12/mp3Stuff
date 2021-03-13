using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Stuff
{
    public class Track : INotifyPropertyChanged
    {
        private string _path;
        private string _fullPath;
        private string _title;
        private string _artist;
        private string _album;
        private string _year;
        private string _genre;
        private TagLib.File _tags;
        private string _directory;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public string Path {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        public string FullPath {
            get => _fullPath;
            set
            {
                if (string.Equals(_fullPath, value)) return;
                if(!string.IsNullOrEmpty(_fullPath) && !string.IsNullOrEmpty(value))
                {
                    File.Move(_fullPath, value);
                }
                _fullPath = value;
                OnPropertyChanged();
            }
        }
        public string Title {
            get => _title;
            set
            {
                if (string.Equals(_title, value)) return;
                _title = value;
                _tags.Tag.Title = value;
                _tags.Save();
                OnPropertyChanged();
            }
        }
        public string Artist {
            get => _artist;
            set
            {
                if (string.Equals(_artist, value)) return;
                _artist = value;
                _tags.Tag.Performers = new string[] { value };
                _tags.Save();
                OnPropertyChanged();
            }
        }
        public string Album {
            get => _album;
            set
            {
                if (string.Equals(_album, value)) return;
                _album = value;
                _tags.Tag.Album = value;
                _tags.Save();
                OnPropertyChanged();
            }
        }
        public string Year {
            get => _year;
            set
            {
                if (string.Equals(_year, value)) return;
                _year = value;
                uint.TryParse(value, out uint year);
                _tags.Tag.Year = year;
                _tags.Save();
                OnPropertyChanged();
            }
        }
        public string Genre {
            get => _genre;
            set
            {
                if (string.Equals(_genre, value)) return;
                _genre = value;
                _tags.Tag.Genres = new string[] { value };
                _tags.Save();
                OnPropertyChanged();
            }
        }
        public string Directory
        {
            get => _directory;
            set
            {
                _directory = value;
            }
        }

        public Track(string path, string fullpath, string title, string artist, string album, string year, string genre, string dir)
        {
            _path = path;
            _fullPath = fullpath;
            _title = title;
            _artist = artist;
            _album = album;
            _year = year;
            _genre = genre;
            _tags = TagLib.File.Create(_fullPath);
            _directory = dir;
        }
        //public Track() { }
        //public Track(string path, string artist, string title, string album)
        //{
        //    _path = path;
        //    _artist = artist;
        //    _title = title;
        //    _album = album;
        //}
    }
}
