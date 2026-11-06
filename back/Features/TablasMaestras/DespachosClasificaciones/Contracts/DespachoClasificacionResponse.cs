namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones.Contracts;

/// <summary>
/// Representa una clasificación de despacho devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la clasificación.</param>
/// <param name="JurisdiccionId">Identificador de la jurisdicción.</param>
/// <param name="MunicipioId">Identificador del municipio.</param>
/// <param name="SubEspecialidadId">Identificador de la sub especialidad, si tiene.</param>
/// <param name="DistritoId">Identificador del distrito.</param>
/// <param name="CircuitoId">Identificador del circuito, si tiene.</param>
/// <param name="Activo">Indica si la clasificación está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record DespachoClasificacionResponse(
    Guid Id,
    Guid JurisdiccionId,
    Guid MunicipioId,
    Guid? SubEspecialidadId,
    Guid DistritoId,
    Guid? CircuitoId,
    bool Activo,
    DateTime FechaCreacionUtc);
