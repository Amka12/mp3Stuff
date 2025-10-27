using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mp3Stuff.Models;

namespace Mp3Stuff.Services
{
    internal class AppDbContext:DbContext
    {
        private readonly string _path = @"./DB.db";
        public DbSet<TrackDb> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source = {_path}");
    }
}
