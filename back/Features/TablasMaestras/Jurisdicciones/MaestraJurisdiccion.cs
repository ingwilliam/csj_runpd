namespace Runpd.Api.Features.TablasMaestras.Jurisdicciones;

/// <summary>
/// Representa una jurisdicción del catálogo maestro.
/// </summary>
public sealed class MaestraJurisdiccion
{
    private MaestraJurisdiccion()
    {
    }

    /// <summary>
    /// Crea una jurisdicción activa con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre de la jurisdicción.</param>
    /// <param name="funcion">Indica si la jurisdicción corresponde a una función.</param>
    public MaestraJurisdiccion(string nombre, bool funcion)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Funcion = funcion;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador de la jurisdicción.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre de la jurisdicción.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene si la jurisdicción corresponde a una función.
    /// </summary>
    public bool Funcion { get; private set; }

    /// <summary>
    /// Obtiene si la jurisdicción está activa.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
