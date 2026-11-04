using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Distritos.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Distritos;

/// <summary>
/// Expone el catálogo de distritos a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/distritos")]
public sealed class DistritosController : ControllerBase
{
    private readonly DistritosService _servicio;

    /// <summary>
    /// Inicializa el controlador de distritos.
    /// </summary>
    /// <param name="servicio">Servicio que consulta los distritos.</param>
    public DistritosController(DistritosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los distritos activos.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de distritos activos, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DistritoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<DistritoResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var distritos = await _servicio.ObtenerActivosAsync(tokenCancelacion);
        return Ok(distritos.Select(ConvertirRespuesta));
    }

    private static DistritoResponse ConvertirRespuesta(MaestraDistrito distrito)
    {
        return new DistritoResponse(
            distrito.Id,
            distrito.Nombre,
            distrito.JurisdiccionId,
            distrito.SubEspecialidadId,
            distrito.Activo,
            distrito.FechaCreacionUtc);
    }
}
