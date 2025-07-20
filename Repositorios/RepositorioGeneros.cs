using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioGeneros : IRepositorioGenero
    {
        private readonly AplicationDbContext _context;

        public RepositorioGeneros(AplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Crear(Genero genero)
        {
            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();
            return genero.Id;
        }

        public async Task<bool> exists(int id)
        {
            return await _context.Generos.AnyAsync(x => x.Id == id);
        }
        public async Task<bool> exists(int id, string nombre)
        {
            return await _context.Generos.AnyAsync(x => x.Id != id && x.Nombre == nombre);
        }

        public async Task<Genero?> GetGenero(int id)
        {
            return await _context.Generos.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Genero>> GetGeneros()
        {
            return await _context.Generos.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task Actualizar(Genero genero)
        {
            _context.Update(genero);
            await _context.SaveChangesAsync();

        }



        public async Task Eliminar(int id)
        {
            await _context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();

        }

        public async Task<List<int>> Existen(List<int> ids)
        {

            return await _context.Generos
                .Where(g => ids.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        }

    }
}
