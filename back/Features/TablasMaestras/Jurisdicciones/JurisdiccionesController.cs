using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Jurisdicciones;

/// <summary>
/// Expone el catálogo de jurisdicciones a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/jurisdicciones")]
public sealed class JurisdiccionesController : ControllerBase
{
    private readonly JurisdiccionesService _servicio;

    /// <summary>
    /// Inicializa el controlador de jurisdicciones.
    /// </summary>
    /// <param name="servicio">Servicio que consulta las jurisdicciones.</param>
    public JurisdiccionesController(JurisdiccionesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista las jurisdicciones activas.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de jurisdicciones activas, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JurisdiccionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<JurisdiccionResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var jurisdicciones = await _servicio.ObtenerActivasAsync(tokenCancelacion);
        return Ok(jurisdicciones.Select(ConvertirRespuesta));
    }

    private static JurisdiccionResponse ConvertirRespuesta(MaestraJurisdiccion jurisdiccion)
    {
        return new JurisdiccionResponse(
            jurisdiccion.Id,
            jurisdiccion.Nombre,
            jurisdiccion.Funcion,
            jurisdiccion.Activo,
            jurisdiccion.FechaCreacionUtc);
    }
}
