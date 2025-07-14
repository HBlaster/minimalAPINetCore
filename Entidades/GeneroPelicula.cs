namespace MinimalApiMovies.Entidades
{
    public class GeneroPelicula
    {
        public int PeliculaId { get; set; }
        public int GeneroId { get; set; }
        public Pelicula Pelicula { get; set; } = null!;
        public Genero Genero { get; set; } = null!;
    }
}
