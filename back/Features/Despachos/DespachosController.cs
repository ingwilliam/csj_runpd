using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.Despachos.Contracts;
using Runpd.Api.Features.Roles;

namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Expone los endpoints HTTP para administrar despachos.
/// La lectura la pueden hacer los roles de consulta; la escritura solo el Administrador de la UDAE.
/// </summary>
[ApiController]
[Route("api/despachos")]
public sealed class DespachosController : ControllerBase
{
    private const string RolesLectura = RolesConocidos.AdministradorUDAE
        + "," + RolesConocidos.Consulta
        + "," + RolesConocidos.AdministradorDelSistema;

    private readonly DespachosService _servicio;

    /// <summary>
    /// Inicializa el controlador de despachos.
    /// </summary>
    /// <param name="servicio">Servicio que ejecuta las operaciones de despachos.</param>
    public DespachosController(DespachosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los despachos paginados, incluidos los inactivos según el filtro.
    /// </summary>
    /// <param name="consulta">Filtros y paginación solicitados.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La página de despachos y los totales del listado.</returns>
    [HttpGet]
    [Authorize(Roles = RolesLectura)]
    [ProducesResponseType(typeof(DespachosPaginadosResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DespachosPaginadosResponse>> Listar(
        [FromQuery] ListarDespachosQuery consulta,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.ObtenerPaginadosAsync(consulta, tokenCancelacion);
        var respuesta = new DespachosPaginadosResponse(
            resultado.Despachos.Select(ConvertirRespuesta).ToList(),
            resultado.Pagina,
            resultado.Tamano,
            resultado.Total,
            resultado.TotalPaginas,
            resultado.TotalDespachos,
            resultado.TotalActivos,
            resultado.TotalInactivos);

        return Ok(respuesta);
    }

    /// <summary>
    /// Obtiene un despacho por su identificador.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El despacho encontrado o una respuesta 404.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = RolesLectura)]
    [ProducesResponseType(typeof(DespachoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DespachoResponse>> ObtenerPorId(Guid id, CancellationToken tokenCancelacion)
    {
        var despacho = await _servicio.ObtenerPorIdAsync(id, tokenCancelacion);
        if (despacho is null)
        {
            return NotFound(new { mensaje = "No se encontró el despacho solicitado." });
        }

        return Ok(ConvertirRespuesta(despacho));
    }

    /// <summary>
    /// Crea un despacho nuevo.
    /// </summary>
    /// <param name="solicitud">Datos del despacho, incluido su estado.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El despacho creado, 400 si una relación no es válida o 409 si el código ya existe.</returns>
    [HttpPost]
    [Authorize(Roles = RolesConocidos.AdministradorUDAE)]
    [ProducesResponseType(typeof(DespachoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DespachoResponse>> Crear(
        DespachoRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.CrearAsync(solicitud, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoOperacionDespacho.Ok => CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Despacho!.Id },
                ConvertirRespuesta(resultado.Despacho!)),
            EstadoOperacionDespacho.CodigoDuplicado => Conflict(
                new { mensaje = "Ya existe un despacho con ese código." }),
            EstadoOperacionDespacho.RelacionInvalida => BadRequest(
                new { mensaje = "Una o más relaciones no existen o están inactivas." }),
            _ => throw new InvalidOperationException(
                $"Estado de creación de despacho no controlado: {resultado.Estado}.")
        };
    }

    /// <summary>
    /// Actualiza los datos de un despacho, incluido su estado.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="solicitud">Nuevos datos del despacho.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El despacho actualizado, 404 si no existe, 400 si una relación no es válida
    /// o 409 si el código ya pertenece a otro despacho.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = RolesConocidos.AdministradorUDAE)]
    [ProducesResponseType(typeof(DespachoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DespachoResponse>> Actualizar(
        Guid id,
        DespachoRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.ActualizarAsync(id, solicitud, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoOperacionDespacho.Ok => Ok(ConvertirRespuesta(resultado.Despacho!)),
            EstadoOperacionDespacho.NoEncontrado => NotFound(
                new { mensaje = "No se encontró el despacho solicitado." }),
            EstadoOperacionDespacho.CodigoDuplicado => Conflict(
                new { mensaje = "Ya existe otro despacho con ese código." }),
            EstadoOperacionDespacho.RelacionInvalida => BadRequest(
                new { mensaje = "Una o más relaciones no existen o están inactivas." }),
            _ => throw new InvalidOperationException(
                $"Estado de actualización de despacho no controlado: {resultado.Estado}.")
        };
    }

    /// <summary>
    /// Cambia el estado activo de un despacho sin eliminar su registro.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="activo">Nuevo estado activo: <see langword="true"/> o <see langword="false"/>.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El despacho actualizado o 404 si no existe.</returns>
    [HttpPatch("{id:guid}/activo/{activo:bool}")]
    [Authorize(Roles = RolesConocidos.AdministradorUDAE)]
    [ProducesResponseType(typeof(DespachoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DespachoResponse>> CambiarActivo(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.CambiarActivoAsync(id, activo, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoOperacionDespacho.Ok => Ok(ConvertirRespuesta(resultado.Despacho!)),
            EstadoOperacionDespacho.NoEncontrado => NotFound(
                new { mensaje = "No se encontró el despacho solicitado." }),
            _ => throw new InvalidOperationException(
                $"Estado de cambio de activo no controlado: {resultado.Estado}.")
        };
    }

    private static DespachoResponse ConvertirRespuesta(Despacho despacho)
    {
        return new DespachoResponse(
            despacho.Id,
            despacho.Codigo,
            despacho.Nombre,
            despacho.Estado,
            despacho.Promiscuo,
            despacho.Direccion,
            despacho.ComplementoDireccion,
            despacho.TelefonoFijo,
            despacho.TelefonoCelular,
            despacho.Correo,
            despacho.HorarioAtencion,
            despacho.EspecialidadId,
            despacho.SubEspecialidadId,
            despacho.MunicipioId,
            despacho.JurisdiccionId,
            despacho.DistritoId,
            despacho.CircuitoId,
            despacho.DistritoSubEspecialidadId,
            despacho.CircuitoSubEspecialidadId,
            despacho.EntidadId,
            despacho.Activo,
            despacho.FechaCreacionUtc);
    }
}
