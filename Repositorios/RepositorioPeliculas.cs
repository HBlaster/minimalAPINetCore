using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Utilidades;

namespace MinimalApiMovies.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        public HttpContext HttpContext;

        public RepositorioPeliculas(AplicationDbContext _context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            context = _context;
            this.mapper = mapper;
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
            return await context.Peliculas.Include(p => p.Comentarios)
                .Include(p => p.GenerosPeliculas).ThenInclude(gp=> gp.Genero)
                .Include(p=>p.ActorPeliculas.OrderBy(a => a.Orden)).ThenInclude(ap => ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {

            var pelicula = await context.Peliculas.Include(p => p.GenerosPeliculas).FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
            {
                throw new Exception($"No se encontró la película con ID {id}");
            }

            var generosPelicullas = generosIds.Select(generoId => new GeneroPelicula { GeneroId = generoId });

            pelicula.GenerosPeliculas = mapper.Map(generosPelicullas, pelicula.GenerosPeliculas);

            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula>actores) {

            for (int i=1; i<actores.Count; i++) {
                actores[i-1].Orden = i;
            }
            var pelicula = await context.Peliculas.Include(p => p.ActorPeliculas)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null) {
                throw new Exception($"No se encontró la película con ID {id}");
            }

            pelicula.ActorPeliculas = mapper.Map(actores, pelicula.ActorPeliculas);
            await context.SaveChangesAsync();

        }

    }
}
