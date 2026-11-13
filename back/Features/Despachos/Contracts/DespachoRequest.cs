using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Despachos.Contracts;

/// <summary>
/// Representa los datos editables de un despacho, usados al crear y al actualizar.
/// </summary>
public sealed class DespachoRequest
{
    /// <summary>
    /// Obtiene o establece el código único del despacho.
    /// </summary>
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    public string Codigo { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el nombre del despacho.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el estado del despacho. El front lo envía; solo se acepta CREADO, MODIFICADO, TRASLADADO o SUPRIMIDO.
    /// </summary>
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [RegularExpression(
        "^(CREADO|MODIFICADO|TRASLADADO|SUPRIMIDO)$",
        ErrorMessage = "El estado debe ser CREADO, MODIFICADO, TRASLADADO o SUPRIMIDO.")]
    public string Estado { get; init; } = string.Empty;

    /// <summary>
    /// Obtiene o establece si el despacho es promiscuo.
    /// </summary>
    public bool Promiscuo { get; init; }

    /// <summary>
    /// Obtiene o establece si el despacho es de descongestión. Por defecto es false.
    /// </summary>
    public bool EsDescongestion { get; init; } = false;

    /// <summary>
    /// Obtiene o establece la dirección del despacho.
    /// </summary>
    [StringLength(255, ErrorMessage = "La dirección no puede superar los 255 caracteres.")]
    public string? Direccion { get; init; }

    /// <summary>
    /// Obtiene o establece el complemento de la dirección.
    /// </summary>
    [StringLength(150, ErrorMessage = "El complemento de dirección no puede superar los 150 caracteres.")]
    public string? ComplementoDireccion { get; init; }

    /// <summary>
    /// Obtiene o establece el teléfono fijo.
    /// </summary>
    [StringLength(50, ErrorMessage = "El teléfono fijo no puede superar los 50 caracteres.")]
    public string? TelefonoFijo { get; init; }

    /// <summary>
    /// Obtiene o establece el teléfono celular.
    /// </summary>
    [StringLength(50, ErrorMessage = "El teléfono celular no puede superar los 50 caracteres.")]
    public string? TelefonoCelular { get; init; }

    /// <summary>
    /// Obtiene o establece el correo electrónico.
    /// </summary>
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string? Correo { get; init; }

    /// <summary>
    /// Obtiene o establece el horario de atención.
    /// </summary>
    [StringLength(150, ErrorMessage = "El horario de atención no puede superar los 150 caracteres.")]
    public string? HorarioAtencion { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador de la especialidad, si aplica.
    /// </summary>
    public Guid? EspecialidadId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador de la sub especialidad, si aplica.
    /// </summary>
    public Guid? SubEspecialidadId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador del municipio. Es obligatorio.
    /// </summary>
    public Guid MunicipioId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador de la jurisdicción. Es obligatorio.
    /// </summary>
    public Guid JurisdiccionId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador del distrito, si aplica.
    /// </summary>
    public Guid? DistritoId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador del circuito, si aplica.
    /// </summary>
    public Guid? CircuitoId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador del distrito asociado a la sub especialidad, si aplica.
    /// </summary>
    public Guid? DistritoSubEspecialidadId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador del circuito asociado a la sub especialidad, si aplica.
    /// </summary>
    public Guid? CircuitoSubEspecialidadId { get; init; }

    /// <summary>
    /// Obtiene o establece el identificador de la entidad, si aplica.
    /// </summary>
    public Guid? EntidadId { get; init; }
}
