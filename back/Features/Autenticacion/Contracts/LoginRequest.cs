using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Autenticacion.Contracts;

/// <summary>
/// Representa las credenciales enviadas para iniciar sesión.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Obtiene el correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El correo electrónico no puede superar los 254 caracteres.")]
    public string CorreoElectronico { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene la contraseña del usuario.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
    public string Contrasena { get; init; } = string.Empty;
}
