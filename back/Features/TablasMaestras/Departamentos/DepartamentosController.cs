using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.TablasMaestras.Departamentos.Contracts;

namespace Runpd.Api.Features.TablasMaestras.Departamentos;

/// <summary>
/// Expone el catálogo de departamentos a usuarios autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/maestras/departamentos")]
public sealed class DepartamentosController : ControllerBase
{
    private readonly DepartamentosService _servicio;

    /// <summary>
    /// Inicializa el controlador de departamentos.
    /// </summary>
    /// <param name="servicio">Servicio que consulta los departamentos.</param>
    public DepartamentosController(DepartamentosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista los departamentos activos.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de departamentos activos, 401 si no hay un token válido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<DepartamentoResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var departamentos = await _servicio.ObtenerActivosAsync(tokenCancelacion);
        return Ok(departamentos.Select(ConvertirRespuesta));
    }

    private static DepartamentoResponse ConvertirRespuesta(MaestraDepartamento departamento)
    {
        return new DepartamentoResponse(
            departamento.Id,
            departamento.Nombre,
            departamento.CodigoDivpola,
            departamento.Activo,
            departamento.FechaCreacionUtc);
    }
}
