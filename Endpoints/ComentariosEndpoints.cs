using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Repositorios;

namespace MinimalApiMovies.Endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapPost("/", Crear);
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comentarios-get"));
            group.MapGet("/{id:int}", ObtenerPorId).WithName("ObtenerComentarioPorId");


            return group;
        }

        static async Task<Results<CreatedAtRoute<ComentarioDTO>, NotFound>> Crear(int peliculaId, CrearComentarioDTO crearComentarioDTO,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {

            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.CreatedAtRoute(comentarioDTO, "ObtenerComentarioPorId", new { id, peliculaId });


        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {

            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }
            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);

        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id, IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {

            var comentario = await repositorioComentarios.ObtenerPorId(id);
            if (comentario is null)
            {
                return TypedResults.NotFound();
            }
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }

    }
}
