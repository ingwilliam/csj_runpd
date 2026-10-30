using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Autenticacion.Contracts;

/// <summary>
/// Representa el correo del usuario que solicita recuperar su contraseña.
/// </summary>
public sealed class RecuperarContrasenaRequest
{
    /// <summary>
    /// Obtiene o establece el correo electrónico de la cuenta.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El correo electrónico no puede superar los 254 caracteres.")]
    public string CorreoElectronico { get; init; } = string.Empty;
}
