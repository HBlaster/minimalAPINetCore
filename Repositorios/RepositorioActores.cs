using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly AplicationDbContext _context;
        public RepositorioActores(AplicationDbContext context)
        {
            this._context = context;

        }
        public async Task<List<Actor>> ObtenerTodos()
        {
            return await _context.Actores.OrderBy(x => x.Nombre).ToListAsync();
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

    }
}

