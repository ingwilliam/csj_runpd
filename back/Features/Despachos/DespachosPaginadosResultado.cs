namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Contiene los despachos de una página y los totales necesarios para la interfaz.
/// </summary>
/// <param name="Despachos">Despachos de la página solicitada.</param>
/// <param name="Pagina">Número de página solicitado.</param>
/// <param name="Tamano">Cantidad máxima de despachos por página.</param>
/// <param name="Total">Despachos que cumplen la búsqueda y el filtro.</param>
/// <param name="TotalPaginas">Cantidad de páginas disponibles para el resultado filtrado.</param>
/// <param name="TotalDespachos">Total de despachos, sin filtros.</param>
/// <param name="TotalPermanentes">Total de despachos activos y no de descongestión.</param>
/// <param name="TotalDescongestion">Total de despachos activos y de descongestión.</param>
/// <param name="TotalSuprimidos">Total de despachos inactivos (suprimidos).</param>
public sealed record DespachosPaginadosResultado(
    IReadOnlyCollection<Despacho> Despachos,
    int Pagina,
    int Tamano,
    int Total,
    int TotalPaginas,
    int TotalDespachos,
    int TotalPermanentes,
    int TotalDescongestion,
    int TotalSuprimidos);
