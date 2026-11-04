namespace Runpd.Api.Features.TablasMaestras.Jurisdicciones.Contracts;

/// <summary>
/// Representa una jurisdicción devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la jurisdicción.</param>
/// <param name="Nombre">Nombre de la jurisdicción.</param>
/// <param name="Funcion">Indica si la jurisdicción corresponde a una función.</param>
/// <param name="Activo">Indica si la jurisdicción está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record JurisdiccionResponse(
    Guid Id,
    string Nombre,
    bool Funcion,
    bool Activo,
    DateTime FechaCreacionUtc);
