namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa una página de usuarios y los totales del listado.
/// </summary>
/// <param name="Items">Usuarios de la página solicitada.</param>
/// <param name="Pagina">Número de página solicitado.</param>
/// <param name="Tamano">Cantidad máxima de usuarios por página.</param>
/// <param name="Total">Usuarios que cumplen la búsqueda y el filtro.</param>
/// <param name="TotalPaginas">Cantidad de páginas disponibles para el resultado filtrado.</param>
/// <param name="TotalUsuarios">Total de usuarios registrados, sin filtros.</param>
/// <param name="TotalActivos">Total de usuarios activos, sin filtros.</param>
/// <param name="TotalInactivos">Total de usuarios inactivos, sin filtros.</param>
public sealed record UsuariosPaginadosResponse(
    IReadOnlyCollection<UsuarioResponse> Items,
    int Pagina,
    int Tamano,
    int Total,
    int TotalPaginas,
    int TotalUsuarios,
    int TotalActivos,
    int TotalInactivos);
