using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Especialidades.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Especialidades;

/// <summary>
/// Expone el catálogo de especialidades a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/especialidades")]
public sealed class EspecialidadesController : ControllerBase
{
    private readonly EspecialidadesService _servicio;

    /// <summary>
    /// Inicializa el controlador de especialidades.
    /// </summary>
    /// <param name="servicio">Servicio que consulta las especialidades.</param>
    public EspecialidadesController(EspecialidadesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista las especialidades activas.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de especialidades activas, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EspecialidadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<EspecialidadResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var especialidades = await _servicio.ObtenerActivasAsync(tokenCancelacion);
        return Ok(especialidades.Select(ConvertirRespuesta));
    }

    private static EspecialidadResponse ConvertirRespuesta(MaestraEspecialidad especialidad)
    {
        return new EspecialidadResponse(
            especialidad.Id,
            especialidad.Nombre,
            especialidad.Descripcion,
            especialidad.Activo,
            especialidad.FechaCreacionUtc);
    }
}
