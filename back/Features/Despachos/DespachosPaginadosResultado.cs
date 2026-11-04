namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Contiene los despachos de una página y los totales necesarios para la interfaz.
/// </summary>
/// <param name="Despachos">Despachos de la página solicitada.</param>
/// <param name="Pagina">Número de página solicitado.</param>
/// <param name="Tamano">Cantidad máxima de despachos por página.</param>
/// <param name="Total">Despachos que cumplen la búsqueda y el filtro.</param>
/// <param name="TotalPaginas">Cantidad de páginas disponibles para el resultado filtrado.</param>
/// <param name="TotalDespachos">Total de despachos registrados, sin filtros.</param>
/// <param name="TotalActivos">Total de despachos activos, sin filtros.</param>
/// <param name="TotalInactivos">Total de despachos inactivos, sin filtros.</param>
public sealed record DespachosPaginadosResultado(
    IReadOnlyCollection<Despacho> Despachos,
    int Pagina,
    int Tamano,
    int Total,
    int TotalPaginas,
    int TotalDespachos,
    int TotalActivos,
    int TotalInactivos);
