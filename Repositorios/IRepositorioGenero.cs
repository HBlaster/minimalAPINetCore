using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public interface IRepositorioGenero
    {
        Task<int> Crear(Genero genero);
        Task<List<Genero>> GetGeneros();
        Task<Genero?> GetGenero(int id);
    }
}
