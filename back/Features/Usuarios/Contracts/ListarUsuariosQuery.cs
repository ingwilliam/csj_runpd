using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa los filtros y la paginación solicitados para listar usuarios.
/// </summary>
public sealed class ListarUsuariosQuery
{
    /// <summary>
    /// Obtiene o establece el número de página solicitado, desde 1.
    /// </summary>
    [Range(1, 1_000_000, ErrorMessage = "La página debe estar entre 1 y 1.000.000.")]
    public int Pagina { get; init; } = 1;

    /// <summary>
    /// Obtiene o establece la cantidad de usuarios por página.
    /// </summary>
    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int Tamano { get; init; } = 20;

    /// <summary>
    /// Obtiene o establece el texto para buscar en nombres, correo o roles.
    /// </summary>
    [StringLength(100, ErrorMessage = "La búsqueda no puede superar los 100 caracteres.")]
    public string? Busqueda { get; init; }

    /// <summary>
    /// Obtiene o establece el estado por el que se filtran los usuarios.
    /// </summary>
    public EstadoUsuarioFiltro Estado { get; init; } = EstadoUsuarioFiltro.Todos;
}
