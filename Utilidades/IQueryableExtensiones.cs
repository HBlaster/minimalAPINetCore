using MinimalApiMovies.DTOs;

namespace MinimalApiMovies.Utilidades
{
    public static class IQueryableExtensiones
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacion)
        {

            return queryable
                .Skip((paginacion.Pagina - 1) * paginacion.RecordsPorPagina)
                .Take(paginacion.RecordsPorPagina);

        }
    }
}
