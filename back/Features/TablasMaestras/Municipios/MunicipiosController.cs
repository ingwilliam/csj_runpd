using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Municipios.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Municipios;

/// <summary>
/// Expone el catálogo de municipios a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/municipios")]
public sealed class MunicipiosController : ControllerBase
{
    private readonly MunicipiosService _servicio;

    /// <summary>
    /// Inicializa el controlador de municipios.
    /// </summary>
    /// <param name="servicio">Servicio que consulta los municipios.</param>
    public MunicipiosController(MunicipiosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los municipios activos de un departamento.
    /// </summary>
    /// <param name="departamentoId">Identificador del departamento. Es obligatorio.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de municipios del departamento, 400 si falta el departamento o 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MunicipioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MunicipioResponse>>> Listar(
        [FromQuery] Guid? departamentoId,
        CancellationToken tokenCancelacion)
    {
        // El catálogo completo supera los mil registros, así que la consulta siempre se acota por departamento.
        if (departamentoId is null || departamentoId == Guid.Empty)
        {
            return BadRequest(new { mensaje = "Debes indicar el departamento con el parámetro departamentoId." });
        }

        var municipios = await _servicio.ObtenerActivosPorDepartamentoAsync(departamentoId.Value, tokenCancelacion);
        return Ok(municipios.Select(ConvertirRespuesta));
    }

    private static MunicipioResponse ConvertirRespuesta(MaestraMunicipio municipio)
    {
        return new MunicipioResponse(
            municipio.Id,
            municipio.Nombre,
            municipio.Tipo,
            municipio.Pdet,
            municipio.CodigoDivpola,
            municipio.DepartamentoId,
            municipio.ConsejoSeccionalId,
            municipio.DireccionSeccionalId,
            municipio.Activo,
            municipio.FechaCreacionUtc);
    }
}
