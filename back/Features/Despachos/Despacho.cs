namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Representa un despacho judicial administrado por la UDAE.
/// </summary>
public sealed class Despacho
{
    private Despacho()
    {
    }

    /// <summary>
    /// Crea un despacho activo con los datos indicados.
    /// </summary>
    /// <param name="datos">Datos validados del despacho.</param>
    public Despacho(DatosDespacho datos)
    {
        Id = Guid.NewGuid();
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
        CopiarDatos(datos);
    }

    /// <summary>
    /// Obtiene el identificador del despacho.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el código único del despacho.
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el nombre del despacho.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el estado del despacho enviado por el cliente: CREADO, MODIFICADO, TRASLADADO o SUPRIMIDO.
    /// </summary>
    public string Estado { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene si el despacho es promiscuo.
    /// </summary>
    public bool Promiscuo { get; private set; }

    /// <summary>
    /// Obtiene la dirección del despacho, si tiene.
    /// </summary>
    public string? Direccion { get; private set; }

    /// <summary>
    /// Obtiene el complemento de la dirección, si tiene.
    /// </summary>
    public string? ComplementoDireccion { get; private set; }

    /// <summary>
    /// Obtiene el teléfono fijo del despacho, si tiene.
    /// </summary>
    public string? TelefonoFijo { get; private set; }

    /// <summary>
    /// Obtiene el teléfono celular del despacho, si tiene.
    /// </summary>
    public string? TelefonoCelular { get; private set; }

    /// <summary>
    /// Obtiene el correo electrónico del despacho, si tiene.
    /// </summary>
    public string? Correo { get; private set; }

    /// <summary>
    /// Obtiene el horario de atención del despacho, si tiene.
    /// </summary>
    public string? HorarioAtencion { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la especialidad, si tiene.
    /// </summary>
    public Guid? EspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la sub especialidad, si tiene.
    /// </summary>
    public Guid? SubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del municipio del despacho.
    /// </summary>
    public Guid MunicipioId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la jurisdicción del despacho.
    /// </summary>
    public Guid JurisdiccionId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del distrito, si tiene.
    /// </summary>
    public Guid? DistritoId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del circuito, si tiene.
    /// </summary>
    public Guid? CircuitoId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del distrito asociado a la sub especialidad, si tiene.
    /// </summary>
    public Guid? DistritoSubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del circuito asociado a la sub especialidad, si tiene.
    /// </summary>
    public Guid? CircuitoSubEspecialidadId { get; private set; }

    /// <summary>
    /// Obtiene el identificador de la entidad, si tiene.
    /// </summary>
    public Guid? EntidadId { get; private set; }

    /// <summary>
    /// Obtiene si el despacho está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del registro en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }

    /// <summary>
    /// Reemplaza los datos editables del despacho. El estado activo solo cambia con <see cref="EstablecerActivo"/>.
    /// </summary>
    /// <param name="datos">Nuevos datos validados del despacho.</param>
    public void Actualizar(DatosDespacho datos)
    {
        CopiarDatos(datos);
    }

    /// <summary>
    /// Establece el estado activo del despacho.
    /// </summary>
    /// <param name="activo">Nuevo estado activo.</param>
    public void EstablecerActivo(bool activo)
    {
        Activo = activo;
    }

    // Copia todos los datos editables; se usa tanto al crear como al actualizar.
    private void CopiarDatos(DatosDespacho datos)
    {
        Codigo = datos.Codigo;
        Nombre = datos.Nombre;
        Estado = datos.Estado;
        Promiscuo = datos.Promiscuo;
        Direccion = datos.Direccion;
        ComplementoDireccion = datos.ComplementoDireccion;
        TelefonoFijo = datos.TelefonoFijo;
        TelefonoCelular = datos.TelefonoCelular;
        Correo = datos.Correo;
        HorarioAtencion = datos.HorarioAtencion;
        EspecialidadId = datos.EspecialidadId;
        SubEspecialidadId = datos.SubEspecialidadId;
        MunicipioId = datos.MunicipioId;
        JurisdiccionId = datos.JurisdiccionId;
        DistritoId = datos.DistritoId;
        CircuitoId = datos.CircuitoId;
        DistritoSubEspecialidadId = datos.DistritoSubEspecialidadId;
        CircuitoSubEspecialidadId = datos.CircuitoSubEspecialidadId;
        EntidadId = datos.EntidadId;
    }
}
