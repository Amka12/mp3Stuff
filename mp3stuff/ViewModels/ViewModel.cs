using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Mp3Stuff.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        const string path = @"F:\Music\test";
        public ICollectionView TracksView { get; set; }

        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        public bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        #region Fields

        #region Files count
        private string _files_count = "Файлов всего: 0";
        public string Files_count
        {
            get => _files_count;
            set => Set(ref _files_count, value);
        }
        #endregion

        #region Tracks
        private ObservableCollection<Track> _tracks = new ObservableCollection<Track>();
        public ObservableCollection<Track> Tracks
        {
            get => _tracks;
            set => Set(ref _tracks, value);
        }
        #endregion

        #region Selected track
        private Track _selectedTrack;
        public Track SelectedTrack
        {
            get => _selectedTrack;
            set => Set(ref _selectedTrack, value);
        }
        #endregion

        #endregion


        #region Commands

        #region CloseAppCommand
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region ScanCommand
        public ICommand ScanCommand { get; }
        private bool CanScanCommandExecute(object p) => true;
        private void OnScanCommandExecuted(object p)
        {
            Tracks.Clear();
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mp3", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var tags = TagLib.File.Create(file.FullName);
                Tracks.Add(new Track(file.Name, file.FullName, tags.Tag.Title, tags.Tag.FirstPerformer, tags.Tag.Album, tags.Tag.Year.ToString(), tags.Tag.FirstGenre, file.DirectoryName));
            }
        }
        #endregion

        #region Rename Command
        public ICommand RenameCommand { get; }
        private bool CanRenameCommandExecute(object p)
        {
            if (SelectedTrack == null) return false;
            if (string.IsNullOrEmpty(SelectedTrack.Title) || string.IsNullOrEmpty(SelectedTrack.Artist)) return false;
            string newName = $"{SelectedTrack.Artist} - {SelectedTrack.Title}.mp3";
            if (string.Equals(SelectedTrack.Path, newName)) return false;
            return true;
        }
        private void OnRenameCommandExecuted(object p)
        {
            string newName = $"{SelectedTrack.Artist} - {SelectedTrack.Title}.mp3";
            if (string.Equals(SelectedTrack.Path, newName)) return;
            SelectedTrack.Path = newName;
            SelectedTrack.FullPath = $"{SelectedTrack.Directory}\\{newName}";
        }
        #endregion

        #region Group by artist command
        public ICommand GroupCommand { get; }
        private bool CanGroupCommandExecute(object p) => true;
        private void OnGroupCommandExecuted(object p)
        {
            TracksView.GroupDescriptions.Clear();
            TracksView.GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
        }
        #endregion

        #endregion

        public ViewModel()
        {
            CloseApplicationCommand = new Commands(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            ScanCommand = new Commands(OnScanCommandExecuted, CanScanCommandExecute);
            RenameCommand = new Commands(OnRenameCommandExecuted, CanRenameCommandExecute);
            TracksView = CollectionViewSource.GetDefaultView(Tracks);
            TracksView.GroupDescriptions.Clear();
            GroupCommand = new Commands(OnGroupCommandExecuted, CanGroupCommandExecute);
        }
    }
}
