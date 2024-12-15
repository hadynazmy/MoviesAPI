using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {
        }
        public DbSet<Genre> Genras { get; set; }
        public DbSet<Movie> Movies { get; set; }

    }
}
