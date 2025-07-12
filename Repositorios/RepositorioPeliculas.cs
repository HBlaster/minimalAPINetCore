using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Utilidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly AplicationDbContext context;
        public HttpContext HttpContext;

        public RepositorioPeliculas(AplicationDbContext _context, IHttpContextAccessor httpContextAccessor)
        {
            context = _context;
            HttpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Peliculas.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Actualizar(Pelicula pelicula)
        {

            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            await context.Peliculas.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }
    }
}
