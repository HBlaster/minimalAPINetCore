using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public interface IRepositorioGenero
    {
        Task<int> Crear(Genero genero);
    }
}
