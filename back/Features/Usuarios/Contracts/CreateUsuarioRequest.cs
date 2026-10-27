using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa los datos necesarios para crear un usuario.
/// </summary>
public sealed class CreateUsuarioRequest
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

    /// <summary>
    /// Obtiene o establece el correo electrónico del usuario.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El correo electrónico no puede superar los 254 caracteres.")]
    public string CorreoElectronico { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la contraseña del usuario.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    public string Contrasena { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece los roles activos que se asignarán al crear el usuario.
    /// Es opcional: si se omite o está vacío, el usuario se crea sin roles.
    /// </summary>
    public IReadOnlyCollection<Guid>? Roles { get; init; }
}
