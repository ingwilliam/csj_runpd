using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades.Contracts;

namespace Runpd.Api.Features.TablasMaestras.SubEspecialidades;

/// <summary>
/// Expone el catálogo de sub especialidades a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/sub-especialidades")]
public sealed class SubEspecialidadesController : ControllerBase
{
    private readonly SubEspecialidadesService _servicio;

    /// <summary>
    /// Inicializa el controlador de sub especialidades.
    /// </summary>
    /// <param name="servicio">Servicio que consulta las sub especialidades.</param>
    public SubEspecialidadesController(SubEspecialidadesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista las sub especialidades activas.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de sub especialidades activas, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubEspecialidadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SubEspecialidadResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var subEspecialidades = await _servicio.ObtenerActivasAsync(tokenCancelacion);
        return Ok(subEspecialidades.Select(ConvertirRespuesta));
    }

    private static SubEspecialidadResponse ConvertirRespuesta(MaestraSubEspecialidad subEspecialidad)
    {
        return new SubEspecialidadResponse(
            subEspecialidad.Id,
            subEspecialidad.EspecialidadId,
            subEspecialidad.Nombre,
            subEspecialidad.Descripcion,
            subEspecialidad.Promiscuo,
            subEspecialidad.Codigo,
            subEspecialidad.Activo,
            subEspecialidad.FechaCreacionUtc);
    }
}
