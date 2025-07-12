namespace MinimalApiMovies.DTOs
{
    public class crearPeliculaDTO
    {
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
