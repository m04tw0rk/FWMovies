using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FW.Movies.Models;
using FW.Movies.DataServices.Interfaces;

namespace FW.Movies.Controllers
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesController : Controller
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService) {
            this.movieService = movieService;
        }


        [HttpGet]
        public IActionResult Get([FromQuery] MovieFilterCriteriaModel model)
        {
            if (model == null || !IsValidFilterCriteria(model))
            {
                return BadRequest();
            }

            var dtos = movieService.Filter(model.Title, model.YearOfRelease, model.Genres);

            if (!dtos.Any())
            {
                return NotFound();
            }

            return Ok(dtos);
        }

        [HttpGet("top5")]
        public IActionResult Get([FromQuery] int? userId)
        {
            var dtos = movieService.Top5MoviesBasedOnRating(userId);

            if (!dtos.Any())
            {
                return NotFound();
            }

            return Ok(dtos);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]MovieRatingRequestModel model)
        {
            if (!(model.Score >= 1M && model.Score <= 5M))
            {
                return BadRequest();
            }

            var success = movieService.AddOrUpdateRating(model.MovieId, model.UserId, model.Score);

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }

        private bool IsValidFilterCriteria(MovieFilterCriteriaModel model)
        {
            return (model.Genres != null && model.Genres.Any()) || !string.IsNullOrEmpty(model.Title) || model.YearOfRelease.HasValue;
        }
    }
}