using Microsoft.EntityFrameworkCore;
using Subtitles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<PhraseTranslation> PhraseTranslations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Phrase>().HasIndex(x => new { x.MovieId, x.SequenceNumber }).IsUnique();
        }
    }
}
