using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entidades;

namespace MinimalApiMovies
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genero>().Property(g => g.Nombre).HasMaxLength(50);
        }

        public DbSet<Genero> Generos { get; set; }
    }
}
