namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Estados disponibles para filtrar el listado de usuarios.
/// </summary>
public enum EstadoUsuarioFiltro
{
    /// <summary>Incluye usuarios activos e inactivos.</summary>
    Todos,

    /// <summary>Incluye solo usuarios activos.</summary>
    Activos,

    /// <summary>Incluye solo usuarios inactivos.</summary>
    Inactivos
}
