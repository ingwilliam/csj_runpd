using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Despachos.Contracts;

/// <summary>
/// Representa los filtros y la paginación solicitados para listar despachos.
/// </summary>
public sealed class ListarDespachosQuery
{
    /// <summary>
    /// Obtiene o establece el número de página solicitado, desde 1.
    /// </summary>
    [Range(1, 1_000_000, ErrorMessage = "La página debe estar entre 1 y 1.000.000.")]
    public int Pagina { get; init; } = 1;

    /// <summary>
    /// Obtiene o establece la cantidad de despachos por página.
    /// </summary>
    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int Tamano { get; init; } = 20;

    /// <summary>
    /// Obtiene o establece el texto para buscar en el código o el nombre.
    /// </summary>
    [StringLength(100, ErrorMessage = "La búsqueda no puede superar los 100 caracteres.")]
    public string? Busqueda { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro de activos e inactivos.
    /// </summary>
    public EstadoDespachoFiltro Estado { get; init; } = EstadoDespachoFiltro.Todos;
}
