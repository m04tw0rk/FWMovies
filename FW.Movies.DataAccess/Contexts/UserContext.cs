using Microsoft.EntityFrameworkCore;
using FW.Movies.DataAccess.Entities;

namespace FW.Movies.DataAccess.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Entities.User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new { Id = 1, FirstName = "Dave", LastName = "Bloggs" });

            modelBuilder.Entity<User>().HasData(new { Id = 2, FirstName = "Jane", LastName = "Bloggs" });
        }
    }
}
