namespace Runpd.Api.Features.TablasMaestras.Departamentos.Contracts;

/// <summary>
/// Representa un departamento devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del departamento.</param>
/// <param name="Nombre">Nombre del departamento.</param>
/// <param name="CodigoDivpola">Código DIVIPOLA del departamento.</param>
/// <param name="Activo">Indica si el departamento está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record DepartamentoResponse(
    Guid Id,
    string Nombre,
    string CodigoDivpola,
    bool Activo,
    DateTime FechaCreacionUtc);
