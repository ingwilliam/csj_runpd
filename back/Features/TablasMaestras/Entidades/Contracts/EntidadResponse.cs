namespace Runpd.Api.Features.TablasMaestras.Entidades.Contracts;

/// <summary>
/// Representa una entidad devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la entidad.</param>
/// <param name="Codigo">Código de la entidad.</param>
/// <param name="Nombre">Nombre de la entidad.</param>
/// <param name="JurisdiccionId">Identificador de la jurisdicción a la que pertenece.</param>
/// <param name="Activo">Indica si la entidad está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record EntidadResponse(
    Guid Id,
    string Codigo,
    string Nombre,
    Guid JurisdiccionId,
    bool Activo,
    DateTime FechaCreacionUtc);
