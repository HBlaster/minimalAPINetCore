using AutoMapper;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;
using MinimalApiMovies.Migrations;

namespace MinimalApiMovies.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Generos
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();

            //Actores
            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Actor, ActorDTO>();

            //Peliculas
            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(p => p.Generos,
                entidad => entidad.MapFrom(p =>
                p.GenerosPeliculas.Select(gp =>
                new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                .ForMember(p => p.Actores, entidad => entidad.MapFrom(p =>
                p.ActorPeliculas.Select(ap =>
                new ActorPeliculaDTO
                {
                    Id = ap.ActorId,
                    Nombre = ap.Actor.Nombre,
                    Personaje = ap.Personaje
                })));

            //Comentarios
            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            //ActorPelicula
            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();





        }
    }
}
