using Microsoft.EntityFrameworkCore;
using Mp3Stuff.Models;

namespace Mp3Stuff.Services;

internal class AppDbContext : DbContext
{
    private readonly string _path = @"./DB.db";
    public DbSet<TrackDb> Tracks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source = {_path}");
    }
}