namespace MinimalApiMovies.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(string? ruta, string Contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);
        async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo)
        {

            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }
    }
}
