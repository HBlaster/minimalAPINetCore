using System.ComponentModel.DataAnnotations;

namespace MinimalApiMovies.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        //[StringLength(50)]
        public string Nombre { get; set; } = null!;
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
    }
}
