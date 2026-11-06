namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones;

/// <summary>
/// Representa la clasificación territorial de un despacho, asociada a una jurisdicción,
/// un municipio y, opcionalmente, a una sub especialidad.
/// </summary>
public sealed class MaestraDespachoClasificacion
{
    private MaestraDespachoClasificacion()
    {
    }

    /// <summary>
    /// Crea una clasificación activa con sus datos iniciales.
    /// </summary>
    /// <param name="jurisdiccionId">Identificador de la jurisdicción.</param>
    /// <param name="municipioId">Identificador del municipio.</param>
    /// <param name="subEspecialidadId">Identificador de la sub especialidad, si aplica.</param>
    /// <param name="distritoId">Identificador del distrito.</param>
    /// <param name="circuitoId">Identificador del circuito, si aplica.</param>
    public MaestraDespachoClasificacion(
        Guid jurisdiccionId,
        Guid municipioId,
        Guid? subEspecialidadId,
        Guid distritoId,
        Guid? circuitoId)
    {
        Id = Guid.NewGuid();
        JurisdiccionId = jurisdiccionId;
        MunicipioId = municipioId;
        SubEspecialidadId = subEspecialidadId;
        DistritoId = distritoId;
        CircuitoId = circuitoId;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la clasificación.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la jurisdicción.
    /// </summary>
    public Guid JurisdiccionId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del municipio.
    /// </summary>
    public Guid MunicipioId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la sub especialidad, si tiene.
    /// </summary>
    public Guid? SubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del distrito.
    /// </summary>
    public Guid DistritoId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del circuito, si tiene.
    /// </summary>
    public Guid? CircuitoId { get; private set; }

    /// <summary>
    /// Obtiene si la clasificación está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
