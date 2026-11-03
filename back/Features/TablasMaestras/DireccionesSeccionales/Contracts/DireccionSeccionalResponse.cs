namespace Runpd.Api.Features.TablasMaestras.DireccionesSeccionales.Contracts;

/// <summary>
/// Representa una dirección seccional devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la dirección seccional.</param>
/// <param name="Nombre">Nombre de la dirección seccional.</param>
/// <param name="Activo">Indica si la dirección seccional está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record DireccionSeccionalResponse(
    Guid Id,
    string Nombre,
    bool Activo,
    DateTime FechaCreacionUtc);
