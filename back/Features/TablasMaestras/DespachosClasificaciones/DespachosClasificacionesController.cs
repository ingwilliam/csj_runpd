using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.DespachosClasificaciones.Contracts;

namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones;

/// <summary>
/// Expone la resolución de la clasificación territorial de los despachos a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/despachos-clasificaciones")]
public sealed class DespachosClasificacionesController : ControllerBase
{
    private readonly DespachosClasificacionesService _servicio;

    /// <summary>
    /// Inicializa el controlador de clasificaciones de despachos.
    /// </summary>
    /// <param name="servicio">Servicio que resuelve la clasificación territorial.</param>
    public DespachosClasificacionesController(DespachosClasificacionesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Resuelve la clasificación general y la específica para una jurisdicción y municipio.
    /// </summary>
    /// <param name="jurisdiccionId">Identificador de la jurisdicción. Es obligatorio.</param>
    /// <param name="municipioId">Identificador del municipio. Es obligatorio.</param>
    /// <param name="subEspecialidadId">Identificador de la sub especialidad, si aplica.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>Las clasificaciones general y específica, 400 si falta un parámetro obligatorio
    /// o 401 si no hay un token válido.</returns>
    [HttpGet("resolver-territorio")]
    [ProducesResponseType(typeof(ResolverTerritorioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResolverTerritorioResponse>> ResolverTerritorio(
        [FromQuery] Guid? jurisdiccionId,
        [FromQuery] Guid? municipioId,
        [FromQuery] Guid? subEspecialidadId,
        CancellationToken tokenCancelacion)
    {
        // La jurisdicción y el municipio son obligatorios para poder resolver el territorio.
        if (jurisdiccionId is null || jurisdiccionId == Guid.Empty)
        {
            return BadRequest(new { mensaje = "Debes indicar la jurisdicción con el parámetro jurisdiccionId." });
        }

        if (municipioId is null || municipioId == Guid.Empty)
        {
            return BadRequest(new { mensaje = "Debes indicar el municipio con el parámetro municipioId." });
        }

        var resultado = await _servicio.ResolverTerritorioAsync(
            jurisdiccionId.Value,
            municipioId.Value,
            subEspecialidadId == Guid.Empty ? null : subEspecialidadId,
            tokenCancelacion);

        return Ok(new ResolverTerritorioResponse(
            resultado.General is null ? null : ConvertirRespuesta(resultado.General),
            resultado.Especifico is null ? null : ConvertirRespuesta(resultado.Especifico)));
    }

    private static DespachoClasificacionResponse ConvertirRespuesta(MaestraDespachoClasificacion clasificacion)
    {
        return new DespachoClasificacionResponse(
            clasificacion.Id,
            clasificacion.JurisdiccionId,
            clasificacion.MunicipioId,
            clasificacion.SubEspecialidadId,
            clasificacion.DistritoId,
            clasificacion.CircuitoId,
            clasificacion.Activo,
            clasificacion.FechaCreacionUtc);
    }
}
