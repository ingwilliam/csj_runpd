namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa un rol asignado a un usuario.
/// </summary>
/// <param name="Id">Identificador del rol.</param>
/// <param name="Nombre">Nombre del rol.</param>
public sealed record RolAsignadoResponse(Guid Id, string Nombre);
