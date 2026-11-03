namespace Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;

/// <summary>
/// Representa una dirección seccional del catálogo maestro.
/// </summary>
public sealed class MaestraDireccionSeccional
{
    private MaestraDireccionSeccional()
    {
    }

    /// <summary>
    /// Crea una dirección seccional activa con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre de la dirección seccional.</param>
    public MaestraDireccionSeccional(string nombre)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la dirección seccional.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre de la dirección seccional.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene si la dirección seccional está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
