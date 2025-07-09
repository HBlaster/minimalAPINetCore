using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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
        static async Task<Ok<List<Genero>>> obtenerGeneros(IRepositorioGenero repositorio)
        {
            var generos = await repositorio.GetGeneros();
            return TypedResults.Ok(generos);
        }

        static async Task<Results<Ok<Genero>, NotFound>> obtenerGeneroId(IRepositorioGenero repositorio, int id)
        {
            var genero = await repositorio.GetGenero(id);
            if (genero == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(genero);
        }

        static async Task<Created<Genero>> crearGenero(Genero genero, IRepositorioGenero repositorio,
            IOutputCacheStore outputCacheStore)
        {
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.Created($"/generos/{id}", genero);
        }

        static async Task<Results<NoContent, NotFound>> actualizarGenero(int id, Genero genero, IRepositorioGenero repositorio, IOutputCacheStore outputCacheStore)
        {

            var exists = await repositorio.exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }

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
