namespace Runpd.Api.Features.TablasMaestras.Entidades;

/// <summary>
/// Representa una entidad del catálogo maestro, asociada a una jurisdicción.
/// </summary>
public sealed class MaestraEntidad
{
    private MaestraEntidad()
    {
    }

    /// <summary>
    /// Crea una entidad activa con sus datos iniciales.
    /// </summary>
    /// <param name="codigo">Código de la entidad.</param>
    /// <param name="nombre">Nombre de la entidad.</param>
    /// <param name="jurisdiccionId">Identificador de la jurisdicción a la que pertenece.</param>
    public MaestraEntidad(string codigo, string nombre, Guid jurisdiccionId)
    {
        Id = Guid.NewGuid();
        Codigo = codigo;
        Nombre = nombre;
        JurisdiccionId = jurisdiccionId;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la entidad.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el código de la entidad.
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el nombre de la entidad.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el identificador de la jurisdicción a la que pertenece.
    /// </summary>
    public Guid JurisdiccionId { get; private set; }

    /// <summary>
    /// Obtiene si la entidad está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
