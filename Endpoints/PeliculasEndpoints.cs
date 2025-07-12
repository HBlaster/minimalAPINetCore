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
            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] crearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio,
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
    }
}
