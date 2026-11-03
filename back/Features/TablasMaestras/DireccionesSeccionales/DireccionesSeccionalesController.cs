using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.DireccionesSeccionales.Contracts;

namespace Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;

/// <summary>
/// Expone el catálogo de direcciones seccionales a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/direcciones-seccionales")]
public sealed class DireccionesSeccionalesController : ControllerBase
{
    private readonly DireccionesSeccionalesService _servicio;

    /// <summary>
    /// Inicializa el controlador de direcciones seccionales.
    /// </summary>
    /// <param name="servicio">Servicio que consulta las direcciones seccionales.</param>
    public DireccionesSeccionalesController(DireccionesSeccionalesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista las direcciones seccionales activas.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de direcciones activas, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DireccionSeccionalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<DireccionSeccionalResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var direcciones = await _servicio.ObtenerActivosAsync(tokenCancelacion);
        return Ok(direcciones.Select(ConvertirRespuesta));
    }

    private static DireccionSeccionalResponse ConvertirRespuesta(MaestraDireccionSeccional direccion)
    {
        return new DireccionSeccionalResponse(
            direccion.Id,
            direccion.Nombre,
            direccion.Activo,
            direccion.FechaCreacionUtc);
    }
}
