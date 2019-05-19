using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FW.Movies.Models
{
    public class MovieRatingRequestModel
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public decimal Score { get; set; }
    }
}
