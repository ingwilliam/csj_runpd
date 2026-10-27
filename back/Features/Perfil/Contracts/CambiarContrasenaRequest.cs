using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Perfil.Contracts;

/// <summary>
/// Representa los datos necesarios para cambiar la contraseña del usuario autenticado.
/// </summary>
public sealed class CambiarContrasenaRequest
{
    /// <summary>
    /// Obtiene o establece la contraseña actual, que se verifica antes de cambiarla.
    /// </summary>
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    [StringLength(100, ErrorMessage = "La contraseña actual no puede superar los 100 caracteres.")]
    public string ContrasenaActual { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la nueva contraseña.
    /// </summary>
    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "La nueva contraseña debe tener entre 8 y 100 caracteres.")]
    public string ContrasenaNueva { get; init; } = string.Empty;
}
