namespace Runpd.Api.Features.TablasMaestras.Municipios;

/// <summary>
/// Representa un municipio del catálogo maestro, identificado por su código DIVIPOLA.
/// </summary>
public sealed class MaestraMunicipio
{
    private MaestraMunicipio()
    {
    }

    /// <summary>
    /// Crea un municipio activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre del municipio.</param>
    /// <param name="codigoDivpola">Código DIVIPOLA del municipio. Se guarda como texto para conservar los ceros iniciales.</param>
    /// <param name="tipo">Tipo de entidad territorial (por ejemplo, "Municipio" o "Isla"), si aplica.</param>
    /// <param name="pdet">Indica si el municipio pertenece a los Programas de Desarrollo con Enfoque Territorial (PDET).</param>
    /// <param name="departamentoId">Identificador del departamento al que pertenece.</param>
    /// <param name="consejoSeccionalId">Identificador del consejo seccional asignado, si lo tiene.</param>
    /// <param name="direccionSeccionalId">Identificador de la dirección seccional asignada, si la tiene.</param>
    public MaestraMunicipio(
        string nombre,
        string codigoDivpola,
        string? tipo,
        bool pdet,
        Guid departamentoId,
        Guid? consejoSeccionalId,
        Guid? direccionSeccionalId)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        CodigoDivpola = codigoDivpola;
        Tipo = tipo;
        Pdet = pdet;
        DepartamentoId = departamentoId;
        ConsejoSeccionalId = consejoSeccionalId;
        DireccionSeccionalId = direccionSeccionalId;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del municipio.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del municipio.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el código DIVIPOLA del municipio.
    /// </summary>
    public string CodigoDivpola { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el tipo de entidad territorial, si aplica.
    /// </summary>
    public string? Tipo { get; private set; }

    /// <summary>
    /// Obtiene si el municipio pertenece a los PDET.
    /// </summary>
    public bool Pdet { get; private set; }

    /// <summary>
    /// Obtiene el identificador del departamento al que pertenece.
    /// </summary>
    public Guid DepartamentoId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del consejo seccional asignado, si lo tiene.
    /// </summary>
    public Guid? ConsejoSeccionalId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la dirección seccional asignada, si la tiene.
    /// </summary>
    public Guid? DireccionSeccionalId { get; private set; }

    /// <summary>
    /// Obtiene si el registro está activo dentro del catálogo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }
}
