namespace Runpd.Api.Features.Roles.Contracts;

/// <summary>
/// Representa un rol devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del rol.</param>
/// <param name="Nombre">Nombre del rol.</param>
/// <param name="Descripcion">Descripción del rol.</param>
/// <param name="Activo">Indica si el rol está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record RolResponse(
    Guid Id,
    string Nombre,
    string? Descripcion,
    bool Activo,
    DateTime FechaCreacionUtc);
