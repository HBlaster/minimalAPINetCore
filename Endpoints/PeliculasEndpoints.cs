using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Repositorios;
using MinimalApiMovies.Servicios;

namespace MinimalApiMovies.Endpoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string contenedor = "peliculas";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapPost("/", Crear).DisableAntiforgery();
            group.MapGet("/", Obtener).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery();
            group.MapDelete("/{id:int}", Borrar);
            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros);
            group.MapPost("/{id:int}/asignaractores", AsignarActores);
            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }
            var id = await repositorio.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);

        }

        static async Task<Ok<List<PeliculaDTO>>> Obtener(IRepositorioPeliculas repositorio, IMapper mapper,
            int pagina = 1, int recordsPorPagina = 10)
        {

            var paginacionDTO = new PaginacionDTO
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina
            };
            var peliculas = await repositorio.ObtenerTodos(paginacionDTO);
            var peliculasDto = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDto);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var pelicula = await repositorio.ObtenerPorId(id);
            if (pelicula == null)
            {
                return TypedResults.NotFound();
            }
            var peliculaDto = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDto);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {

            var peliculaDB = await repositorio.ObtenerPorId(id);
            if (peliculaDB == null)
            {
                return TypedResults.NotFound();
            }
            var peliculaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaActualizar.Id = id;
            peliculaActualizar.Poster = peliculaDB.Poster;
            if (crearPeliculaDTO.Poster is not null)
            {
                peliculaActualizar.Poster = await almacenadorArchivos.Editar(peliculaDB.Poster, contenedor, crearPeliculaDTO.Poster);
            }
            await repositorio.Actualizar(peliculaActualizar);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPeliculas repositorio, IOutputCacheStore outputCacheStore,
            IAlmacenadorArchivos almacenadorArchivos)
        {

            var peliculaDB = await repositorio.ObtenerPorId(id);
            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }
            await repositorio.Eliminar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();


        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds, IRepositorioPeliculas repositorioPeliculas,
            IRepositorioGenero repositorioGenero)
        {

            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();
            if (generosIds.Count != 0)
            {
                generosExistentes = await repositorioGenero.Existen(generosIds);
            }
            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"Los siguientes géneros no existen: {string.Join(", ", generosNoExistentes)}");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();

        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresDTO,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper) {

            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }
            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(x => x.ActorId).ToList();
            if (actoresIds.Count != 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }
            if (actoresExistentes.Count != actoresIds.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los siguientes actores no existen: {string.Join(", ", actoresNoExistentes)}");
            }
            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsignarActores(id, actores);
            return TypedResults.NoContent();
        }
    }
}
