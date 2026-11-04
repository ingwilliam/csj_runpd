namespace Runpd.Api.Features.Despachos.Contracts;

/// <summary>
/// Filtros disponibles para el listado de despachos según su estado activo.
/// </summary>
public enum EstadoDespachoFiltro
{
    /// <summary>Incluye despachos activos e inactivos.</summary>
    Todos,

    /// <summary>Incluye solo despachos activos.</summary>
    Activos,

    /// <summary>Incluye solo despachos inactivos.</summary>
    Inactivos
}
