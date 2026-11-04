namespace Runpd.Api.Features.TablasMaestras.Especialidades;

/// <summary>
/// Representa una especialidad del catálogo maestro.
/// </summary>
public sealed class MaestraEspecialidad
{
    private MaestraEspecialidad()
    {
    }

    /// <summary>
    /// Crea una especialidad activa con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre de la especialidad.</param>
    /// <param name="descripcion">Descripción opcional de la especialidad.</param>
    public MaestraEspecialidad(string nombre, string? descripcion)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la especialidad.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre de la especialidad.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene la descripción de la especialidad, si tiene.
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Obtiene si la especialidad está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
