using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.ConsejosSeccionales.Contracts;

namespace Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;

/// <summary>
/// Expone el catálogo de consejos seccionales a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/consejos-seccionales")]
public sealed class ConsejosSeccionalesController : ControllerBase
{
    private readonly ConsejosSeccionalesService _servicio;

    /// <summary>
    /// Inicializa el controlador de consejos seccionales.
    /// </summary>
    /// <param name="servicio">Servicio que consulta los consejos seccionales.</param>
    public ConsejosSeccionalesController(ConsejosSeccionalesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los consejos seccionales activos.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de consejos activos, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsejoSeccionalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConsejoSeccionalResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var consejos = await _servicio.ObtenerActivosAsync(tokenCancelacion);
        return Ok(consejos.Select(ConvertirRespuesta));
    }

    private static ConsejoSeccionalResponse ConvertirRespuesta(MaestraConsejoSeccional consejo)
    {
        return new ConsejoSeccionalResponse(
            consejo.Id,
            consejo.Nombre,
            consejo.Activo,
            consejo.FechaCreacionUtc);
    }
}
