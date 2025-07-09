using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Repositorios;
using System.Text.RegularExpressions;

namespace MinimalApiMovies.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group) {

            group.MapGet("/", obtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", obtenerGeneroId);
            group.MapPost("/", crearGenero);
            group.MapPut("/{id:int}", actualizarGenero);
            group.MapDelete("/{id:int}", eliminarGenero);
            return group;
        }
        static async Task<Ok<List<GeneroDTO>>> obtenerGeneros(IRepositorioGenero repositorio)
        {
            var generos = await repositorio.GetGeneros();
            var generosDTO = generos.Select(x => new GeneroDTO {Id = x.Id, Nombre = x.Nombre }).ToList();
            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> obtenerGeneroId(IRepositorioGenero repositorio, int id)
        {
            var genero = await repositorio.GetGenero(id);
            if (genero == null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = new GeneroDTO
            {
                Id = genero.Id,
                Nombre = genero.Nombre
            };
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Created<GeneroDTO>> crearGenero(CrearGeneroDTO Ogenero, IRepositorioGenero repositorio,
            IOutputCacheStore outputCacheStore)
        {
            var genero = new Genero
            {
                Nombre = Ogenero.Nombre
            };
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = new GeneroDTO
            {
                Id = id,
                Nombre = genero.Nombre
            };

            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> actualizarGenero(int id, CrearGeneroDTO generoDTO, IRepositorioGenero repositorio, IOutputCacheStore outputCacheStore)
        {

            var exists = await repositorio.exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }

            var genero = new Genero
            {
                Id = id,
                Nombre = generoDTO.Nombre
            };

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> eliminarGenero(int id, IRepositorioGenero repositorio, IOutputCacheStore outputCacheStore)
        {

            var exists = await repositorio.exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
