namespace MinimalApiMovies.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; }
        private int recordsPorPagina = 10;
        private readonly int cantidadMaximaRecordsPorPagina = 50;
        public int RecordsPorPagina
        {
            get => recordsPorPagina;
            set => recordsPorPagina = (value > cantidadMaximaRecordsPorPagina) ? cantidadMaximaRecordsPorPagina : value;
        }
    }
}
