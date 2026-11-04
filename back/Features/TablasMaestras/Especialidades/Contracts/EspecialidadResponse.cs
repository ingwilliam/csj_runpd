namespace Runpd.Api.Features.TablasMaestras.Especialidades.Contracts;

/// <summary>
/// Representa una especialidad devuelta por la API.
/// </summary>
/// <param name="Id">Identificador de la especialidad.</param>
/// <param name="Nombre">Nombre de la especialidad.</param>
/// <param name="Descripcion">Descripción de la especialidad, si tiene.</param>
/// <param name="Activo">Indica si la especialidad está activa.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record EspecialidadResponse(
    Guid Id,
    string Nombre,
    string? Descripcion,
    bool Activo,
    DateTime FechaCreacionUtc);
