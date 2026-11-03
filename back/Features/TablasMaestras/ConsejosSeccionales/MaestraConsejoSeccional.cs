namespace Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;

/// <summary>
/// Representa un consejo seccional del catálogo maestro.
/// </summary>
public sealed class MaestraConsejoSeccional
{
    private MaestraConsejoSeccional()
    {
    }

    /// <summary>
    /// Crea un consejo seccional activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre del consejo seccional.</param>
    public MaestraConsejoSeccional(string nombre)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del consejo seccional.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del consejo seccional.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene si el consejo seccional está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
