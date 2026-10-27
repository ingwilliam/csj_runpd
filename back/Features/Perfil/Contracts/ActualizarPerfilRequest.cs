using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Perfil.Contracts;

/// <summary>
/// Representa los datos personales que el usuario puede editar en su perfil.
/// No incluye correo electrónico, roles ni estado activo.
/// </summary>
public sealed class ActualizarPerfilRequest
{
    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    [Required(ErrorMessage = "El primer nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El primer nombre no puede superar los 100 caracteres.")]
    public string PrimerNombre { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario.
    /// </summary>
    [StringLength(100, ErrorMessage = "El segundo nombre no puede superar los 100 caracteres.")]
    public string? SegundoNombre { get; init; }

    /// <summary>
    /// Obtiene o establece el primer apellido del usuario.
    /// </summary>
    [Required(ErrorMessage = "El primer apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El primer apellido no puede superar los 100 caracteres.")]
    public string PrimerApellido { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el segundo apellido del usuario.
    /// </summary>
    [StringLength(100, ErrorMessage = "El segundo apellido no puede superar los 100 caracteres.")]
    public string? SegundoApellido { get; init; }
}
