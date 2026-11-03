namespace Runpd.Api.Features.TablasMaestras.ConsejosSeccionales.Contracts;

/// <summary>
/// Representa un consejo seccional devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del consejo seccional.</param>
/// <param name="Nombre">Nombre del consejo seccional.</param>
/// <param name="Activo">Indica si el consejo seccional está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record ConsejoSeccionalResponse(
    Guid Id,
    string Nombre,
    bool Activo,
    DateTime FechaCreacionUtc);
