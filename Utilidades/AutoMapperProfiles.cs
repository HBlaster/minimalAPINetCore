using AutoMapper;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();

        }
    }
}
