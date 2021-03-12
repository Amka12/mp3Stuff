using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string _ganre;

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
                _fullPath = value;
                OnPropertyChanged();
            }
        }
        public string Title {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public string Artist {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged();
            }
        }
        public string Album {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged();
            }
        }
        public string Year {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }
        public string Ganre {
            get => _ganre;
            set
            {
                _ganre = value;
                OnPropertyChanged();
            }
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
