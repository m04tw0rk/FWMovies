using FW.Movies.Controllers;
using FW.Movies.DataServices.Dtos;
using FW.Movies.DataServices.Interfaces;
using FW.Movies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace FW.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerTests
    {
        private MoviesController controller;
        private Mock<IMovieService> movieServiceMock;

        [TestInitialize]
        public void SetUp()
        {
            movieServiceMock = new Mock<IMovieService>(MockBehavior.Strict);

            controller = new MoviesController(movieServiceMock.Object);
        }

        [TestCleanup]
        public void Teardoown()
        {
            movieServiceMock.VerifyAll();
        }

        [TestMethod]
        public void Get_ModelIsNull_BadRequestReturned()
        {
            var result = controller.Get((MovieFilterCriteriaModel)null);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Get_ModelHasNullProperties_BadRequestReturned()
        {
            var model = new MovieFilterCriteriaModel();
            var result = controller.Get(model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Get_GetsAMovie_MovieReturned()
        {
            var movieTitle = "Movie";
            var model = new MovieFilterCriteriaModel { Title = movieTitle };

            movieServiceMock.Setup(x => x.Filter(model.Title, null, new int[0])).Returns(new List<MovieDto> { new MovieDto { Title = "Movie" } });

            var result = controller.Get(model) as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(1, ((IList<MovieDto>)result.Value).Count);
            Assert.AreEqual(movieTitle, ((IList<MovieDto>)result.Value).First().Title);
        }

        [TestMethod]
        public void Get_NoMoviesAreReturned_MovieReturned()
        {
            var movieTitle = "Movie";
            var model = new MovieFilterCriteriaModel { Title = movieTitle };

            movieServiceMock.Setup(x => x.Filter(model.Title, null, new int[0])).Returns(new List<MovieDto> { });

            var result = controller.Get(model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Get_UserIdIsNotSuppiled_ReturnsMovies()
        {
            var movieTitle = "Movie";
             
            movieServiceMock.Setup(x => x.Top5MoviesBasedOnRating(null)).Returns(new List<MovieDto> { new MovieDto { Title = "Movie" } });

            var result = controller.Get((int?)null) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(1, ((IList<MovieDto>)result.Value).Count);
            Assert.AreEqual(movieTitle, ((IList<MovieDto>)result.Value).First().Title);
        }

        [TestMethod]
        public void Get_NoMoviesAreReturned_ReturnsEmptyCollection()
        {
            var movieTitle = "Movie";

            movieServiceMock.Setup(x => x.Top5MoviesBasedOnRating(null)).Returns(new List<MovieDto> { });

            var result = controller.Get((int?)null);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Post_InvalidScoreOfTenBeingPosted_BadRequestReturned()
        {
            var result = controller.Post(new MovieRatingRequestModel { Score = 10M });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Post_InvalidScoreOfZeroBeingPosted_BadRequestReturned()
        {
            var result = controller.Post(new MovieRatingRequestModel { Score = 0M });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Post_ServiceSuccessfullyCalled_OkReturned()
        {
            var movieId = 101;
            var userId = 101;
            var score = 5M;
            movieServiceMock.Setup(x => x.AddOrUpdateRating(movieId, userId, score)).Returns(true);

            var result = controller.Post(new MovieRatingRequestModel { Score = score, MovieId = movieId, UserId = userId });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Post_ServiceSuccessfullyCalled_BadRequestReturned()
        {
            var movieId = 101;
            var userId = 101;
            var score = 5M;
            movieServiceMock.Setup(x => x.AddOrUpdateRating(movieId, userId, score)).Returns(false);

            var result = controller.Post(new MovieRatingRequestModel { Score = score, MovieId = movieId, UserId = userId });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}
