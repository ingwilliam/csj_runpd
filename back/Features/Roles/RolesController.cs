using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.Roles.Contracts;

namespace Runpd.Api.Features.Roles;

/// <summary>
/// Expone los endpoints HTTP para administrar roles.
/// </summary>
[ApiController]
[Authorize(Roles = RolesConocidos.AdministradorDelSistema)]
[Route("api/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly RolesService _servicio;

    /// <summary>
    /// Inicializa el controlador de roles.
    /// </summary>
    /// <param name="servicio">Servicio que ejecuta las operaciones de roles.</param>
    public RolesController(RolesService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista todos los roles, incluidos los desactivados.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de roles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RolResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RolResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var roles = await _servicio.ObtenerTodosAsync(tokenCancelacion);
        return Ok(roles.Select(ConvertirRespuesta));
    }

    /// <summary>
    /// Crea un rol nuevo.
    /// </summary>
    /// <param name="solicitud">Datos del rol que se va a crear.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El rol creado o un conflicto si el nombre ya está registrado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RolResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RolResponse>> Crear(
        CreateRolRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var rol = await _servicio.CrearAsync(solicitud, tokenCancelacion);
        if (rol is null)
        {
            return Conflict(new { mensaje = "Ya existe un rol con ese nombre." });
        }

        return CreatedAtAction(nameof(Listar), ConvertirRespuesta(rol));
    }

    /// <summary>
    /// Cambia el estado activo de un rol sin eliminar su registro.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="activo">Nuevo estado activo: <see langword="true"/> o <see langword="false"/>.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El rol actualizado, 404 si no existe o 409 si el rol es del sistema o tiene usuarios asignados.</returns>
    [HttpPatch("{id:guid}/activo/{activo:bool}")]
    [ProducesResponseType(typeof(RolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RolResponse>> CambiarActivo(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.CambiarActivoAsync(id, activo, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoCambiarActivoRol.Ok => Ok(ConvertirRespuesta(resultado.Rol!)),
            EstadoCambiarActivoRol.NoEncontrado => NotFound(
                new { mensaje = "No se encontró el rol solicitado." }),
            EstadoCambiarActivoRol.EnUso => Conflict(
                new { mensaje = "El rol es del sistema o tiene usuarios asignados y no puede desactivarse." }),
            _ => throw new InvalidOperationException(
                $"Estado de cambio de activo de rol no controlado: {resultado.Estado}.")
        };
    }

    private static RolResponse ConvertirRespuesta(Rol rol)
    {
        return new RolResponse(
            rol.Id,
            rol.Nombre,
            rol.Descripcion,
            rol.Activo,
            rol.FechaCreacionUtc);
    }
}
