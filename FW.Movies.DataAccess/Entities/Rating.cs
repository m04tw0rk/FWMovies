namespace FW.Movies.DataAccess.Entities
{
    public class Rating
    {
        //public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public decimal Score { get; set; }  
    }
}
