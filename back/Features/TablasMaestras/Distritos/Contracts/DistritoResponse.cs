namespace Runpd.Api.Features.TablasMaestras.Distritos.Contracts;

/// <summary>
/// Representa un distrito devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del distrito.</param>
/// <param name="Nombre">Nombre del distrito.</param>
/// <param name="JurisdiccionId">Identificador de la jurisdicción a la que pertenece.</param>
/// <param name="SubEspecialidadId">Identificador de la sub especialidad asociada, si tiene.</param>
/// <param name="Activo">Indica si el distrito está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record DistritoResponse(
    Guid Id,
    string Nombre,
    Guid JurisdiccionId,
    Guid? SubEspecialidadId,
    bool Activo,
    DateTime FechaCreacionUtc);
