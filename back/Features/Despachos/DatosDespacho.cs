namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Agrupa los datos editables de un despacho ya normalizados, para crearlo o actualizarlo.
/// </summary>
/// <param name="Codigo">Código único del despacho.</param>
/// <param name="Nombre">Nombre del despacho.</param>
/// <param name="Estado">Estado del despacho: CREADO, MODIFICADO, TRASLADADO o SUPRIMIDO.</param>
    /// <param name="Promiscuo">Indica si el despacho es promiscuo.</param>
    /// <param name="EsDescongestion">Indica si el despacho es de descongestión.</param>
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
public sealed record DatosDespacho(
    string Codigo,
    string Nombre,
    string Estado,
    bool Promiscuo,
    bool EsDescongestion,
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
    Guid? EntidadId);
