using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Mp3Stuff.Models;
using Mp3Stuff.Services;
using File = TagLib.File;

namespace Mp3Stuff.ViewModels;

public class ViewModel : INotifyPropertyChanged
{
    private const string _path = @"F:\Music\test";
    private readonly List<Track> _baseTrackList = new();
    private readonly LastFMService _lastFm = new();

    public ViewModel()
    {
        CloseApplicationCommand = new Commands(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
        ScanCommand = new Commands(OnScanCommandExecuted, CanScanCommandExecute);
        RenameCommand = new Commands(OnRenameCommandExecuted, CanRenameCommandExecute);
        RenameAllCommand = new Commands(OnRenameAllCommandExecuted, CanRenameAllCommandExecute);
        CopyFromLastFM = new Commands(OnCopyFromLastFMExecuted, CanCopyFromLastFMExecute);
        SelectTrackCommand = new AsyncCommands(OnSelectTrackCommandExecuted, CanSelectTrackCommandExecute);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
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

    private List<Track> _tracks = new();

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
        set
        {
            Set(ref _selectedTrack, value);
            //LastFMAlbum = _lastFM.GetAlbumInfo(value);
        }
    }

    #endregion

    #region Last FM Album

    private string _lastFMAlbum;

    public string LastFMAlbum
    {
        get => _lastFMAlbum;
        set => Set(ref _lastFMAlbum, value);
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
            if (value != "Без фильтра") Tracks = Tracks.Where(i => i.Artist == value).ToList();
            Set(ref _selectedArtist, value);
        }
    }

    #endregion

    #endregion


    #region Commands

    #region CloseAppCommand

    public ICommand CloseApplicationCommand { get; }

    private bool CanCloseApplicationCommandExecute(object p)
    {
        return true;
    }

    private void OnCloseApplicationCommandExecuted(object p)
    {
        Application.Current.Shutdown();
    }

    #endregion

    #region ScanCommand

    public ICommand ScanCommand { get; }

    private bool CanScanCommandExecute(object p)
    {
        return true;
    }

    private void OnScanCommandExecuted(object p)
    {
        Tracks.Clear();
        _baseTrackList.Clear();
        using (var context = new AppDbContext())
        {
            context.Tracks.RemoveRange(context.Tracks);

            string[] extensions = { ".mp3", ".flac" };
            var di = new DirectoryInfo(_path);
            var files = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
            foreach (var file in files)
            {
                var tags = File.Create(file.FullName);
                _baseTrackList.Add(new Track(file.Name, file.FullName, tags.Tag.Title, tags.Tag.FirstPerformer, tags.Tag.Album, tags.Tag.Year.ToString(), tags.Tag.FirstGenre, file.DirectoryName));
                context.Tracks.Add(new TrackDb()
                {
                    Artist = tags.Tag.FirstPerformer,
                    Title = tags.Tag.Title,
                    Album = tags.Tag.Album,
                    Genre = tags.Tag.FirstGenre,
                    Path = file.FullName
                });
            }
            context.SaveChanges();
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
        var newName = $"{SelectedTrack.Artist} - {SelectedTrack.Title}.mp3";
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

    private bool CanRenameAllCommandExecute(object p)
    {
        if (string.Equals(SelectedArtist, "Без фильтра")) return false;
        if (Tracks is null || Tracks.Count == 0) return false;
        return true;
    }

    private void OnRenameAllCommandExecuted(object p)
    {
        foreach (var track in Tracks) RenameTrackFile(track);
    }

    #endregion

    #region Copy from Last.FM command

    public ICommand CopyFromLastFM { get; }

    private bool CanCopyFromLastFMExecute(object p)
    {
        if (SelectedTrack == null || string.Equals(SelectedTrack.Album, LastFMAlbum) || string.IsNullOrEmpty(LastFMAlbum)
            || string.Equals(LastFMAlbum, "album not found") || string.Equals(LastFMAlbum, "track not found")) return false;
        return true;
    }

    private void OnCopyFromLastFMExecuted(object p)
    {
        if (string.Equals(SelectedTrack.Album, LastFMAlbum)) return;
        SelectedTrack.Album = LastFMAlbum;
    }

    #endregion

    #region SelectTrackCommand

    public ICommand SelectTrackCommand { get; }
    private bool CanSelectTrackCommandExecute(object p) => true;

    private async Task OnSelectTrackCommandExecuted(object p)
    {
        if(p is null) return;
        if(p is not  Track) return;
        var track = (Track)p;
        SelectedTrack = track;
        LastFMAlbum = await _lastFm.GetAlbumInfoAsync(track);
    }
    #endregion

    #endregion

    #region Methods

    private void RefreshArtistList()
    {
        if (Artists is null) Artists = new List<string>();
        Artists.Clear();
        Artists = Tracks.Select(k => k.Artist).Distinct().OrderBy(u => u).ToList();
        Artists.RemoveAll(s => string.IsNullOrEmpty(s));
        Artists.Insert(0, "Без фильтра");
        SelectedArtist = Artists[0];
    }

    private void RenameTrackFile(Track track)
    {
        var newName = $"{track.Artist} - {track.Title}.mp3";
        if (string.Equals(track.Path, newName)) return;
        track.Path = newName;
        track.FullPath = $"{track.Directory}\\{newName}";
    }

    #endregion
}