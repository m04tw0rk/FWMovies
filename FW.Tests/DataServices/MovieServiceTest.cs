using FW.Movies.DataAccess.Contexts;
using FW.Movies.DataAccess.Entities;
using FW.Movies.DataServices;
using FW.Movies.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FW.Tests.DataServices
{
    [TestClass]
    public class MovieServiceTest
    {
        IMovieService movieService;
        MovieContext movieContext;
        UserContext userContext;

        [TestInitialize]
        public void SetUp()
        {
            var movieOptions = new DbContextOptionsBuilder<MovieContext>()
                .UseInMemoryDatabase(databaseName: "Movies")
                .Options;

            var userOptions = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "Users")
                .Options;

            movieContext = new MovieContext(movieOptions);
            userContext = new UserContext(userOptions);

            movieService = new MovieService(movieContext, userContext);
        }

        [TestCleanup]
        public void Teardoown()
        {
            movieContext.Database.EnsureDeleted();
            userContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void Filter_FindMovieGivenTitle_Success()
        {
            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movie = new Movie { Title = "Frozen 2", Id = movieId };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.Filter(movieTitle, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(movieId, result.First().Id);
            Assert.AreEqual(movieTitle, result.First().Title);
        }

        [TestMethod]
        public void Filter_FindMovieGivenYearAndGenre_Success()
        {
            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movieYear = 2019;
            var genre = Genre.RomanticComedy;
            var movie = new Movie { Title = "Frozen 2", Id = movieId, YearOfRelease = movieYear, Genre = genre };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.Filter(null, 2019, new int[] { (int)genre });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(movieId, result.First().Id);
            Assert.AreEqual(movieTitle, result.First().Title);
            Assert.AreEqual(movieYear, result.First().YearOfRelease);
        }

        [TestMethod]
        public void Filter_FindMovieGivenGenre_Success()
        {
            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movieYear = 2019;
            var genre = Genre.Animation;
            var movie = new Movie { Title = "Frozen 2", Id = movieId, YearOfRelease = movieYear, Genre = genre };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.Filter(null, null, new int[] { (int)genre });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(movieId, result.First().Id);
            Assert.AreEqual(movieTitle, result.First().Title);
            Assert.AreEqual(movieYear, result.First().YearOfRelease);
        }

        [TestMethod]
        public void Filter_DoNotFindMovie_EmptyCollectionReturned()
        {
            var result = movieService.Filter(null, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Top5MoviesBasedOnRating_ReturnsTop5_Success()
        {
            // Clear out Seeded data
            foreach (var entity in movieContext.Movies)
                movieContext.Movies.Remove(entity);
            movieContext.SaveChanges();

            var user1 = new User { Id = 101 };
            var user2 = new User { Id = 102 };

            var movie1 = new Movie { Title = "Movie 1", Id = 101, Ratings = new List<Rating> { new Rating {MovieId = 101, UserId = user1.Id, Score = 2.31M }  } };
            var movie2 = new Movie { Title = "Movie 2", Id = 102, Ratings = new List<Rating> { new Rating { MovieId = 102, UserId = user1.Id, Score = 3M } } };
            var movie3 = new Movie { Title = "Movie 3", Id = 103, Ratings = new List<Rating> { new Rating { MovieId = 103, UserId = user1.Id, Score = 4.6M }, new Rating { MovieId = 103, UserId = user2.Id, Score = 5M } } };
            var movie4 = new Movie { Title = "Movie 4", Id = 104, Ratings = new List<Rating> { new Rating { MovieId = 104, UserId = user1.Id, Score = 1.5M } } };
            var movie5 = new Movie { Title = "Movie 5", Id = 105, Ratings = new List<Rating> { new Rating { MovieId = 105, UserId = user1.Id, Score = 3M } } };
            var movie6 = new Movie { Title = "Movie 6", Id = 106, Ratings = new List<Rating> { new Rating { MovieId = 106, UserId = user1.Id, Score = 5M }, new Rating { MovieId = 106, UserId = user2.Id, Score = 1M } } };

            userContext.Add(user1);
            userContext.Add(user2);

            userContext.SaveChanges();

            movieContext.Movies.Add(movie1);
            movieContext.Movies.Add(movie2);
            movieContext.Movies.Add(movie3);
            movieContext.Movies.Add(movie4);
            movieContext.Movies.Add(movie5);
            movieContext.Movies.Add(movie6);

            movieContext.SaveChanges();

            var result = movieService.Top5MoviesBasedOnRating(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(5M, result[0].AverageRating);
            Assert.AreEqual(3M, result[1].AverageRating);
            Assert.AreEqual(3M, result[2].AverageRating);
            Assert.AreEqual(3M, result[3].AverageRating);
            Assert.AreEqual(2.5M, result[4].AverageRating);
        }

        [TestMethod]
        public void Top5MoviesBasedOnRating_ReturnsTop5GivenAUserId_Success()
        {
            // Clear out Seeded data
            foreach (var entity in movieContext.Movies)
                movieContext.Movies.Remove(entity);
            movieContext.SaveChanges();

            var user1 = new User { Id = 101 };
            var user2 = new User { Id = 102 };

            var movie1 = new Movie { Title = "Movie 1", Id = 101, Ratings = new List<Rating> { new Rating { MovieId = 101, UserId = user1.Id, Score = 2.31M } } };
            var movie2 = new Movie { Title = "Movie 2", Id = 102, Ratings = new List<Rating> { new Rating { MovieId = 102, UserId = user1.Id, Score = 3M } } };
            var movie3 = new Movie { Title = "Movie 3", Id = 103, Ratings = new List<Rating> { new Rating { MovieId = 103, UserId = user1.Id, Score = 4.6M }, new Rating { MovieId = 103, UserId = user2.Id, Score = 5M } } };
            var movie4 = new Movie { Title = "Movie 4", Id = 104, Ratings = new List<Rating> { new Rating { MovieId = 104, UserId = user1.Id, Score = 1.5M } } };
            var movie5 = new Movie { Title = "Movie 5", Id = 105, Ratings = new List<Rating> { new Rating { MovieId = 105, UserId = user1.Id, Score = 3M } } };
            var movie6 = new Movie { Title = "Movie 6", Id = 106, Ratings = new List<Rating> { new Rating { MovieId = 106, UserId = user1.Id, Score = 5M }, new Rating { MovieId = 106, UserId = user2.Id, Score = 1M } } };

            userContext.Add(user1);
            userContext.Add(user2);

            userContext.SaveChanges();

            movieContext.Movies.Add(movie1);
            movieContext.Movies.Add(movie2);
            movieContext.Movies.Add(movie3);
            movieContext.Movies.Add(movie4);
            movieContext.Movies.Add(movie5);
            movieContext.Movies.Add(movie6);

            movieContext.SaveChanges();

            var result = movieService.Top5MoviesBasedOnRating(user1.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(5M, result[0].AverageRating);
            Assert.AreEqual(4.5M, result[1].AverageRating);
            Assert.AreEqual(3M, result[2].AverageRating);
            Assert.AreEqual(3M, result[3].AverageRating);
            Assert.AreEqual(2.5M, result[4].AverageRating);
        }

        [TestMethod]
        public void AddOrUpdateRating_AddUserRating_Success()
        {
            var user1 = new User { Id = 101 };
            userContext.Add(user1);
            userContext.SaveChanges();

            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movie = new Movie { Title = "Frozen 2", Id = movieId };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.AddOrUpdateRating(movie.Id, user1.Id, 5M);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddOrUpdateRating_UpdateExistingUserRating_Success()
        {
            var user1 = new User { Id = 101 };
            userContext.Add(user1);
            userContext.SaveChanges();

            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movie = new Movie { Title = "Frozen 2", Id = movieId, Ratings = new List<Rating> { new Rating { MovieId = 101, UserId = user1.Id, Score = 2.31M } } };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.AddOrUpdateRating(movie.Id, user1.Id, 5M);

            Assert.IsTrue(result);

            var rehydratedMovie = movieService.Filter(movieTitle, null, new int[0]).FirstOrDefault();

            Assert.IsNotNull(rehydratedMovie);
            Assert.AreEqual(5M, rehydratedMovie.AverageRating);
        }

        [TestMethod]
        public void AddOrUpdateRating_MovieNotFound_FalseReturned()
        {
            var result = movieService.AddOrUpdateRating(0, 0, 5M);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddOrUpdateRating_UserNotFound_FalseReturned()
        {
            var movieTitle = "Frozen 2";
            var movieId = 234;
            var movie = new Movie { Title = "Frozen 2", Id = movieId };

            movieContext.Movies.Add(movie);
            movieContext.SaveChanges();

            var result = movieService.AddOrUpdateRating(movie.Id, 0, 5M);

            Assert.IsFalse(result);
        }

    }
}
