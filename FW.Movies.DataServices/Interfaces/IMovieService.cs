using FW.Movies.DataServices.Dtos;
using System.Collections.Generic;

namespace FW.Movies.DataServices.Interfaces
{
    public interface IMovieService
    {
        IList<MovieDto> Filter(string title, int? yearOfRelease, int[] genres);
        IList<MovieDto> Top5MoviesBasedOnRating(int? userId);
        bool AddOrUpdateRating(int movieId, int userId, decimal score);
    }
}
