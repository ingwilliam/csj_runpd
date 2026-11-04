namespace Runpd.Api.Features.TablasMaestras.SubEspecialidades.Contracts;

/// <summary>
/// Representa una sub especialidad devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la sub especialidad.</param>
/// <param name="EspecialidadId">Identificador de la especialidad a la que pertenece, si aplica.</param>
/// <param name="Nombre">Nombre de la sub especialidad.</param>
/// <param name="Descripcion">Descripción de la sub especialidad, si tiene.</param>
/// <param name="Promiscuo">Indica si la sub especialidad es promiscua.</param>
/// <param name="Codigo">Código de la sub especialidad, si tiene.</param>
/// <param name="Activo">Indica si la sub especialidad está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record SubEspecialidadResponse(
    Guid Id,
    Guid? EspecialidadId,
    string Nombre,
    string? Descripcion,
    bool Promiscuo,
    string? Codigo,
    bool Activo,
    DateTime FechaCreacionUtc);
