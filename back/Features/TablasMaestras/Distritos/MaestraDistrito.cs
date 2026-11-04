namespace Runpd.Api.Features.TablasMaestras.Distritos;

/// <summary>
/// Representa un distrito del catálogo maestro, asociado a una jurisdicción y opcionalmente a una sub especialidad.
/// </summary>
public sealed class MaestraDistrito
{
    private MaestraDistrito()
    {
    }

    /// <summary>
    /// Crea un distrito activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre del distrito.</param>
    /// <param name="jurisdiccionId">Identificador de la jurisdicción a la que pertenece.</param>
    /// <param name="subEspecialidadId">Identificador de la sub especialidad asociada, si aplica.</param>
    public MaestraDistrito(string nombre, Guid jurisdiccionId, Guid? subEspecialidadId)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        JurisdiccionId = jurisdiccionId;
        SubEspecialidadId = subEspecialidadId;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del distrito.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del distrito.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el identificador de la jurisdicción a la que pertenece.
    /// </summary>
    public Guid JurisdiccionId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la sub especialidad asociada, si tiene.
    /// </summary>
    public Guid? SubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene si el distrito está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
