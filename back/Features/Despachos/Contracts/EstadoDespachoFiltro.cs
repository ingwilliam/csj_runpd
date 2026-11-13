namespace Runpd.Api.Features.Despachos.Contracts;

/// <summary>
/// Filtros disponibles para el listado de despachos.
/// Cada categoría es mutuamente excluyente: un despacho suprimido no se cuenta
/// ni en Permanentes ni en De Descongestión.
/// </summary>
public enum EstadoDespachoFiltro
{
    /// <summary>Incluye todos los despachos, sin filtro.</summary>
    Todos,

    /// <summary>Incluye solo despachos activos y no de descongestión (permanentes).</summary>
    Permanentes,

    /// <summary>Incluye solo despachos activos y de descongestión.</summary>
    Descongestion,

    /// <summary>Incluye solo despachos inactivos (suprimidos).</summary>
    Suprimidos
}
