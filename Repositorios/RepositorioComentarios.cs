﻿using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly AplicationDbContext context;

        public RepositorioComentarios(AplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<List<Comentario>> ObtenerTodos(int peliculaId)
        {

            return await context.Comentarios
                .Where(c => c.PeliculaId == peliculaId)
                .ToListAsync();
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await context.Comentarios
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<int> Crear(Comentario comentario)
        {
            context.Comentarios.Add(comentario);
            await context.SaveChangesAsync();
            return comentario.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Comentarios.AnyAsync(c => c.Id == id);
        }

        public async Task Actualizar(Comentario comentario)
        {
            context.Update(comentario);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Comentarios
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
