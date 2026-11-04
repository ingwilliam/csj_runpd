using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Entidades.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Entidades;

/// <summary>
/// Expone el catálogo de entidades a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/entidades")]
public sealed class EntidadesController : ControllerBase
{
    private readonly EntidadesService _servicio;

    /// <summary>
    /// Inicializa el controlador de entidades.
    /// </summary>
    /// <param name="servicio">Servicio que consulta las entidades.</param>
    public EntidadesController(EntidadesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista las entidades activas.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de entidades activas, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EntidadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<EntidadResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var entidades = await _servicio.ObtenerActivasAsync(tokenCancelacion);
        return Ok(entidades.Select(ConvertirRespuesta));
    }

    private static EntidadResponse ConvertirRespuesta(MaestraEntidad entidad)
    {
        return new EntidadResponse(
            entidad.Id,
            entidad.Codigo,
            entidad.Nombre,
            entidad.JurisdiccionId,
            entidad.Activo,
            entidad.FechaCreacionUtc);
    }
}
