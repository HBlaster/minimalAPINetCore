using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Utilidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly AplicationDbContext _context;
        private readonly HttpContext httpContext;

        public RepositorioActores(AplicationDbContext context, IHttpContextAccessor httpContextAccesor)
        {
            this._context = context;
            httpContext = httpContextAccesor.HttpContext!;
        }
        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Actores.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
        }
        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await _context.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int> Crear(Actor actor)
        {
            _context.Actores.Add(actor);
            await _context.SaveChangesAsync();
            return actor.Id;
        }
        public async Task<bool> Existe(int id)
        {
            return await _context.Actores.AnyAsync(a => a.Id == id);
        }

        public async Task Actualizar(Actor actor)
        {

            _context.Update(actor);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {

            await _context.Actores.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Actor>> ObtenerPorNombre(string Nombre)
        {
            return await _context.Actores
                .Where(x => x.Nombre.Contains(Nombre)).OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<List<int>> Existen(List<int>ids) {
            return await _context.Actores
                .Where(x => ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
        }

    }
}

