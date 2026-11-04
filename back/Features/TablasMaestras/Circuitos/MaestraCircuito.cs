namespace Runpd.Api.Features.TablasMaestras.Circuitos;

/// <summary>
/// Representa un circuito del catálogo maestro, asociado a un distrito y opcionalmente a una sub especialidad.
/// </summary>
public sealed class MaestraCircuito
{
    private MaestraCircuito()
    {
    }

    /// <summary>
    /// Crea un circuito activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre del circuito.</param>
    /// <param name="distritoId">Identificador del distrito al que pertenece.</param>
    /// <param name="subEspecialidadId">Identificador de la sub especialidad asociada, si aplica.</param>
    public MaestraCircuito(string nombre, Guid distritoId, Guid? subEspecialidadId)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        DistritoId = distritoId;
        SubEspecialidadId = subEspecialidadId;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del circuito.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del circuito.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el identificador del distrito al que pertenece.
    /// </summary>
    public Guid DistritoId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la sub especialidad asociada, si tiene.
    /// </summary>
    public Guid? SubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene si el circuito está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
