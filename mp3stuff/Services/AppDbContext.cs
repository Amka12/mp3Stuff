using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mp3Stuff.Models;
using System;

namespace Mp3Stuff.Services;

internal class AppDbContext : DbContext
{
    public DbSet<TrackDb> Tracks { get; set; }

    //private readonly string _path = @"./DB.dat";
    public string DbPath { get; }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "app.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}