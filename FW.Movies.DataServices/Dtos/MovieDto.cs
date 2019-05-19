namespace FW.Movies.DataServices.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public decimal AverageRating { get; set; }
    }
}
