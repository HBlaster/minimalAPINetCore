using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Repositorios;

namespace MinimalApiMovies.Endpoints
{
    public static class ActoresEndpoints
    {
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {

            group.MapPost("/", Crear).DisableAntiforgery();
            return group;
        }

        static async Task<Created<ActorDTO>> Crear([FromForm] CrearActorDTO crearActorDTO, IRepositorioActores repositorio, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {

            var actor = mapper.Map<Actor>(crearActorDTO);
            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actores/{id}", actorDTO);

        }
    }
}
