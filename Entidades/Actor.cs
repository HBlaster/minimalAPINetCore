﻿namespace MinimalApiMovies.Entidades
{
    public class Actor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        public List<ActorPelicula> ActorPeliculas { get; set; } = new List<ActorPelicula>();
    }
}
