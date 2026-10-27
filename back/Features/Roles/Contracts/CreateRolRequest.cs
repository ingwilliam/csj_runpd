using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Roles.Contracts;

/// <summary>
/// Representa los datos necesarios para crear un rol.
/// </summary>
public sealed class CreateRolRequest
{
    /// <summary>
    /// Obtiene o establece el nombre del rol.
    /// </summary>
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la descripción del rol.
    /// </summary>
    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
    public string? Descripcion { get; init; }
}
