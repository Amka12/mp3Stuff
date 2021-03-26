﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mp3Stuff.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string path = @"F:\Music\test";
        private List<Track> _baseTrackList = new List<Track>();

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
        private void RefreshArtistList()
        {
            Artists = Tracks.Select(k => k.Artist).Distinct().OrderBy(u => u).ToList();
            Artists.RemoveAll(s => string.IsNullOrEmpty(s));
            Artists.Insert(0, "Без фильтра");
        }
        private void RenameTrackFile(Track track)
        {
            string newName = $"{track.Artist} - {track.Title}.mp3";
            if (string.Equals(track.Path, newName)) return;
            track.Path = newName;
            track.FullPath = $"{track.Directory}\\{newName}";
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
        private List<Track> _tracks = new List<Track>();
        public List<Track> Tracks
        {
            get => _tracks;
            set => Set(ref _tracks, value);
        }
        #endregion

        #region Artists list
        private List<string> _artists;
        public List<string> Artists
        {
            get => _artists;
            set => Set(ref _artists, value);
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

        #region Selected artist
        private string _selectedArtist;
        public string SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                Tracks = _baseTrackList;
                if (value != "Без фильтра")
                {
                    Tracks = Tracks.Where(i => i.Artist == value).ToList();
                }
                Set(ref _selectedArtist, value);
            }
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
            _baseTrackList.Clear();
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mp3", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var tags = TagLib.File.Create(file.FullName);
                _baseTrackList.Add(new Track(file.Name, file.FullName, tags.Tag.Title, tags.Tag.FirstPerformer, tags.Tag.Album, tags.Tag.Year.ToString(), tags.Tag.FirstGenre, file.DirectoryName));
            }
            Tracks = _baseTrackList;
            RefreshArtistList();
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
            RenameTrackFile(SelectedTrack);
        }
        #endregion

        #region Rename all command
        public ICommand RenameAllCommand { get; }
        private bool CanRenameAllCommandExecute(object p) => true;
        private void OnRenameAllCommandExecuted(object p)
        {
            foreach (var track in Tracks)
            {
                RenameTrackFile(track);
            }
        }
        #endregion

        #endregion

        public ViewModel()
        {
            CloseApplicationCommand = new Commands(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            ScanCommand = new Commands(OnScanCommandExecuted, CanScanCommandExecute);
            RenameCommand = new Commands(OnRenameCommandExecuted, CanRenameCommandExecute);
            RenameAllCommand = new Commands(OnRenameAllCommandExecuted, CanRenameAllCommandExecute);
        }
    }
}
