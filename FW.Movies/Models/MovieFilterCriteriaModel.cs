namespace FW.Movies.Models
{
    public class MovieFilterCriteriaModel
    {
        public MovieFilterCriteriaModel()
        {
            Genres = new int[0];
        }

        public string Title { get; set; }
        public int? YearOfRelease { get; set; }
        public int[] Genres { get; set; }
    }
}
