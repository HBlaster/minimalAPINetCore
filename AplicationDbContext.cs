using Microsoft.EntityFrameworkCore;

namespace MinimalApiMovies
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
