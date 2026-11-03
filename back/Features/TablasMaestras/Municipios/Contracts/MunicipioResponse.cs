namespace Runpd.Api.Features.TablasMaestras.Municipios.Contracts;

/// <summary>
/// Representa un municipio devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del municipio.</param>
/// <param name="Nombre">Nombre del municipio.</param>
/// <param name="Tipo">Tipo de entidad territorial, si aplica.</param>
/// <param name="Pdet">Indica si el municipio pertenece a los PDET.</param>
/// <param name="CodigoDivpola">Código DIVIPOLA del municipio.</param>
/// <param name="DepartamentoId">Identificador del departamento al que pertenece.</param>
/// <param name="ConsejoSeccionalId">Identificador del consejo seccional asignado, si lo tiene.</param>
/// <param name="DireccionSeccionalId">Identificador de la dirección seccional asignada, si la tiene.</param>
/// <param name="Activo">Indica si el registro está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record MunicipioResponse(
    Guid Id,
    string Nombre,
    string? Tipo,
    bool Pdet,
    string CodigoDivpola,
    Guid DepartamentoId,
    Guid? ConsejoSeccionalId,
    Guid? DireccionSeccionalId,
    bool Activo,
    DateTime FechaCreacionUtc);
