namespace Runpd.Api.Features.TablasMaestras.Circuitos.Contracts;

/// <summary>
/// Representa un circuito devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del circuito.</param>
/// <param name="Nombre">Nombre del circuito.</param>
/// <param name="DistritoId">Identificador del distrito al que pertenece.</param>
/// <param name="SubEspecialidadId">Identificador de la sub especialidad asociada, si tiene.</param>
/// <param name="Activo">Indica si el circuito está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record CircuitoResponse(
    Guid Id,
    string Nombre,
    Guid DistritoId,
    Guid? SubEspecialidadId,
    bool Activo,
    DateTime FechaCreacionUtc);
