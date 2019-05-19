using FW.Movies.DataAccess.Contexts;
using FW.Movies.DataServices.Interfaces;
using System.Linq;
using FW.Movies.DataAccess.Entities;
using System.Collections.Generic;
using System;
using FW.Movies.DataServices.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FW.Movies.DataServices
{
    public class MovieService : IMovieService
    {
        private readonly MovieContext movieContext;
        private readonly UserContext userContext;

        public MovieService(MovieContext movieContext, UserContext userContext)
        {
            this.movieContext = movieContext;
            this.userContext = userContext;
        }

        public IList<MovieDto> Filter(string title, int? yearOfRelease, int[] genres)
        {
            var movies = movieContext.Movies
                .Include(x => x.Ratings)
                .Where(m => (!string.IsNullOrEmpty(title) && m.Title.ToLowerInvariant().Contains(title.ToLowerInvariant()) )
                                             || (yearOfRelease.HasValue && m.YearOfRelease == yearOfRelease) );

            if (genres!= null && genres.Any())
            {
                if (!movies.Any())
                {
                    movies = movieContext.Movies;
                }

                movies = movies.Where(m => genres.Contains((int)m.Genre));
            }

            return MapToMovieDto(movies);
        }

        public IList<MovieDto> Top5MoviesBasedOnRating(int? userId)
        {
            var movies = movieContext.Movies.Include(x => x.Ratings).AsQueryable();

            if (userId.HasValue)
            {
                var userRating = movieContext.Movies.SelectMany(x => x.Ratings).Where(x => x.UserId == userId).ToList();

                return movies.Where(m => userRating.Any(x => x.MovieId == m.Id))
                    .OrderByDescending(m => GetAverageRating(userRating.Where(x => x.MovieId == m.Id).ToList()))
                    .ThenBy(m => m.Title)
                    .Take(5)
                    .Select(x => new MovieDto
                    {
                        Id = x.Id,
                        RunningTime = x.RunningTime,
                        Title = x.Title,
                        YearOfRelease = x.YearOfRelease,
                        AverageRating = GetAverageRating(userRating.Where(y => y.MovieId == x.Id).ToList())
                    }).ToList();
            }
            
            movies = movies.OrderByDescending(m => GetAverageRating(m.Ratings))
                            .ThenBy(m => m.Title)
                            .Take(5);

            return MapToMovieDto(movies);
        }

        public bool AddOrUpdateRating(int movieId, int userId, decimal score)
        {
            var movie = movieContext.Movies.Include(x => x.Ratings).FirstOrDefault(x => x.Id == movieId);
            
            if (movie == null)
            {
                return false;
            }

            var user = userContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return false;
            }

            var userRating = movie.Ratings.FirstOrDefault(x => x.UserId == userId);

            if (userRating == null)
            {
                userRating = new Rating { Score = score, MovieId = movieId, UserId = userId };

                movie.Ratings.Add(userRating);
            }
            else
            {
                userRating.Score = score;
            }
            
            movieContext.SaveChanges();

            return true;
        }

        private IList<MovieDto> MapToMovieDto(IQueryable<Movie> items)
        {
            return items.Select(x => new MovieDto
            {
                Id = x.Id,
                RunningTime = x.RunningTime,
                Title = x.Title,
                YearOfRelease = x.YearOfRelease,
                AverageRating = GetAverageRating(x.Ratings)
            }).ToList();
        }

        private decimal GetAverageRating(IList<Rating> ratings)
        {
            if (ratings.Any())
            {
                return Math.Round(ratings.Average(r => r.Score) * 2, MidpointRounding.AwayFromZero) / 2;
            }

            return 0M;
        }

    }
}
