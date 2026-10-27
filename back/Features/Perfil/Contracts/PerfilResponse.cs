namespace Runpd.Api.Features.Perfil.Contracts;

/// <summary>
/// Representa los datos del perfil del usuario autenticado.
/// </summary>
/// <param name="Id">Identificador del usuario.</param>
/// <param name="PrimerNombre">Primer nombre del usuario.</param>
/// <param name="SegundoNombre">Segundo nombre del usuario, si lo tiene.</param>
/// <param name="PrimerApellido">Primer apellido del usuario.</param>
/// <param name="SegundoApellido">Segundo apellido del usuario, si lo tiene.</param>
/// <param name="CorreoElectronico">Correo electrónico del usuario; no se puede cambiar desde el perfil.</param>
/// <param name="Roles">Nombres de los roles activos asignados al usuario.</param>
public sealed record PerfilResponse(
    Guid Id,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    string CorreoElectronico,
    IReadOnlyCollection<string> Roles);
