using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FW.Movies.DataAccess.Interfaces
{
    public interface IMovieContext
    {
        DbSet<Entities.Movie> Movies { get; set; }
    }
}
