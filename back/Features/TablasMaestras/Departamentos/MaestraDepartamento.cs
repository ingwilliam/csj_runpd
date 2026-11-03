namespace Runpd.Api.Features.TablasMaestras.Departamentos;

/// <summary>
/// Representa un departamento del catálogo maestro, identificado por su código DIVIPOLA.
/// </summary>
public sealed class MaestraDepartamento
{
    private MaestraDepartamento()
    {
    }

    /// <summary>
    /// Crea un departamento activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre del departamento.</param>
    /// <param name="codigoDivpola">Código DIVIPOLA del departamento. Se guarda como texto para conservar los ceros iniciales.</param>
    public MaestraDepartamento(string nombre, string codigoDivpola)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        CodigoDivpola = codigoDivpola;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del departamento.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del departamento.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el código DIVIPOLA del departamento.
    /// </summary>
    public string CodigoDivpola { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene si el departamento está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
