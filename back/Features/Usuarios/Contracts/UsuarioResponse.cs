namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa un usuario devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del usuario.</param>
/// <param name="PrimerNombre">Primer nombre del usuario.</param>
/// <param name="SegundoNombre">Segundo nombre del usuario, si lo tiene.</param>
/// <param name="PrimerApellido">Primer apellido del usuario.</param>
/// <param name="SegundoApellido">Segundo apellido del usuario, si lo tiene.</param>
/// <param name="CorreoElectronico">Correo electrónico del usuario.</param>
/// <param name="Activo">Indica si el usuario está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
/// <param name="Roles">Roles activos asignados al usuario.</param>
public sealed record UsuarioResponse(
    Guid Id,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    string CorreoElectronico,
    bool Activo,
    DateTime FechaCreacionUtc,
    IReadOnlyCollection<RolAsignadoResponse> Roles);
