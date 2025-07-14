using MinimalApiMovies.Entidades;

namespace MinimalApiMovies.Repositorios
{
    public interface IRepositorioGenero
    {
        Task<int> Crear(Genero genero);
        Task<List<Genero>> GetGeneros();
        Task<Genero?> GetGenero(int id);
        Task<bool> exists(int id);
        Task Actualizar(Genero genero);
        Task Eliminar(int id);
        Task<List<int>> Existen(List<int> ids);
    }
}
