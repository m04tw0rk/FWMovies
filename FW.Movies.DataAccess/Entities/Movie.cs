using System.Collections.Generic;

namespace FW.Movies.DataAccess.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public IList<Rating> Ratings { get; set; }
        public Genre Genre { get; set; }
    }

    public enum Genre
    {
        Documentary = 0,
        Action = 1,
        Thriller = 2,
        Romantic = 3,
        RomanticComedy = 4,
        Comedy = 5,
        SciFi = 6,
        Animation = 7
    }
}
