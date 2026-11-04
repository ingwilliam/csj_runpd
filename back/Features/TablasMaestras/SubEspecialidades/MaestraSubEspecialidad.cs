namespace Runpd.Api.Features.TablasMaestras.SubEspecialidades;

/// <summary>
/// Representa una sub especialidad del catálogo maestro, asociada opcionalmente a una especialidad.
/// </summary>
public sealed class MaestraSubEspecialidad
{
    private MaestraSubEspecialidad()
    {
    }

    /// <summary>
    /// Crea una sub especialidad activa con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre de la sub especialidad.</param>
    /// <param name="especialidadId">Identificador de la especialidad a la que pertenece, si aplica.</param>
    /// <param name="descripcion">Descripción opcional de la sub especialidad.</param>
    /// <param name="codigo">Código opcional de la sub especialidad.</param>
    /// <param name="promiscuo">Indica si la sub especialidad es promiscua.</param>
    public MaestraSubEspecialidad(
        string nombre,
        Guid? especialidadId,
        string? descripcion,
        string? codigo,
        bool promiscuo)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        EspecialidadId = especialidadId;
        Descripcion = descripcion;
        Codigo = codigo;
        Promiscuo = promiscuo;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la sub especialidad.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la especialidad a la que pertenece, si aplica.
    /// </summary>
    public Guid? EspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el nombre de la sub especialidad.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene la descripción de la sub especialidad, si tiene.
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Obtiene si la sub especialidad es promiscua.
    /// </summary>
    public bool Promiscuo { get; private set; }

    /// <summary>
    /// Obtiene el código de la sub especialidad, si tiene. No es único.
    /// </summary>
    public string? Codigo { get; private set; }

    /// <summary>
    /// Obtiene si la sub especialidad está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
