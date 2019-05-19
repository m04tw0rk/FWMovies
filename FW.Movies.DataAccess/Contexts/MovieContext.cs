using Microsoft.EntityFrameworkCore;
using FW.Movies.DataAccess.Entities;
using FW.Movies.DataAccess.Interfaces;
using System.Collections.Generic;

namespace FW.Movies.DataAccess.Contexts
{
    public class MovieContext : DbContext, IMovieContext
    {
        public MovieContext(DbContextOptions<MovieContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>().HasKey(k => new { k.MovieId, k.UserId });

            modelBuilder.Entity<Rating>().HasData(new { MovieId = 1, UserId = 1, Score = 4.75M });

            modelBuilder.Entity<Rating>().HasData(new { MovieId = 3, UserId = 1, Score = 3M });

            modelBuilder.Entity<Rating>().HasData(new { MovieId = 4, UserId = 1, Score = 2.6M });

            modelBuilder.Entity<Rating>().HasData(new { MovieId = 4, UserId = 2, Score = 4.21M });

            modelBuilder.Entity<Rating>().HasData(new { MovieId = 2, UserId = 2, Score = 1M });


            //TODO: add more ratings

            modelBuilder.Entity<Movie>().Property(m => m.Genre).HasConversion<int>();

            modelBuilder.Entity<Movie>().HasData(new { Id = 1, Title = "John Wick 3", YearOfRelease = 2019, RunningTime = 120, Genre = Genre.Action,  });

            modelBuilder.Entity<Movie>().HasData(new { Id = 2, Title = "Spiderman: Far From Home", YearOfRelease = 2019, RunningTime = 110, Genre = Genre.Action, });

            modelBuilder.Entity<Movie>().HasData(new { Id = 3, Title = "Avengers: Endgame", YearOfRelease = 2019, RunningTime = 200, Genre = Genre.SciFi, });

            modelBuilder.Entity<Movie>().HasData(new { Id = 4, Title = "When Harry Met Sally", YearOfRelease = 1989, RunningTime = 96, Genre = Genre.RomanticComedy, });

        }
    }
}
