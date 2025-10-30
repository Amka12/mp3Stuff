namespace Mp3Stuff.Models;

internal class TrackDb
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
    public string Path { get; set; }
    public string FullPath { get; set; }
    public string Album { get; set; }
}