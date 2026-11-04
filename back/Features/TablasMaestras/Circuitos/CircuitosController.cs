using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Circuitos.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Circuitos;

/// <summary>
/// Expone el catálogo de circuitos a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/circuitos")]
public sealed class CircuitosController : ControllerBase
{
    private readonly CircuitosService _servicio;

    /// <summary>
    /// Inicializa el controlador de circuitos.
    /// </summary>
    /// <param name="servicio">Servicio que consulta los circuitos.</param>
    public CircuitosController(CircuitosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los circuitos activos.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de circuitos activos, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CircuitoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CircuitoResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var circuitos = await _servicio.ObtenerActivosAsync(tokenCancelacion);
        return Ok(circuitos.Select(ConvertirRespuesta));
    }

    private static CircuitoResponse ConvertirRespuesta(MaestraCircuito circuito)
    {
        return new CircuitoResponse(
            circuito.Id,
            circuito.Nombre,
            circuito.DistritoId,
            circuito.SubEspecialidadId,
            circuito.Activo,
            circuito.FechaCreacionUtc);
    }
}
