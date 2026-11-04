namespace Runpd.Api.Features.Despachos.Contracts;

/// <summary>
/// Representa un despacho devuelto por la API.
/// </summary>
/// <param name="Id">Identificador del despacho.</param>
/// <param name="Codigo">Código único del despacho.</param>
/// <param name="Nombre">Nombre del despacho.</param>
/// <param name="Estado">Estado del despacho.</param>
/// <param name="Promiscuo">Indica si el despacho es promiscuo.</param>
/// <param name="Direccion">Dirección del despacho, si tiene.</param>
/// <param name="ComplementoDireccion">Complemento de la dirección, si tiene.</param>
/// <param name="TelefonoFijo">Teléfono fijo, si tiene.</param>
/// <param name="TelefonoCelular">Teléfono celular, si tiene.</param>
/// <param name="Correo">Correo electrónico, si tiene.</param>
/// <param name="HorarioAtencion">Horario de atención, si tiene.</param>
/// <param name="EspecialidadId">Especialidad, si tiene.</param>
/// <param name="SubEspecialidadId">Sub especialidad, si tiene.</param>
/// <param name="MunicipioId">Municipio del despacho.</param>
/// <param name="JurisdiccionId">Jurisdicción del despacho.</param>
/// <param name="DistritoId">Distrito, si tiene.</param>
/// <param name="CircuitoId">Circuito, si tiene.</param>
/// <param name="DistritoSubEspecialidadId">Distrito asociado a la sub especialidad, si tiene.</param>
/// <param name="CircuitoSubEspecialidadId">Circuito asociado a la sub especialidad, si tiene.</param>
/// <param name="EntidadId">Entidad, si tiene.</param>
/// <param name="Activo">Indica si el despacho está activo.</param>
/// <param name="FechaCreacionUtc">Fecha de creación en UTC.</param>
public sealed record DespachoResponse(
    Guid Id,
    string Codigo,
    string Nombre,
    string Estado,
    bool Promiscuo,
    string? Direccion,
    string? ComplementoDireccion,
    string? TelefonoFijo,
    string? TelefonoCelular,
    string? Correo,
    string? HorarioAtencion,
    Guid? EspecialidadId,
    Guid? SubEspecialidadId,
    Guid MunicipioId,
    Guid JurisdiccionId,
    Guid? DistritoId,
    Guid? CircuitoId,
    Guid? DistritoSubEspecialidadId,
    Guid? CircuitoSubEspecialidadId,
    Guid? EntidadId,
    bool Activo,
    DateTime FechaCreacionUtc);
