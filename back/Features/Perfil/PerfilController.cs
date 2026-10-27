using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.Perfil.Contracts;
using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Features.Perfil;

/// <summary>
/// Expone los endpoints del perfil del usuario autenticado.
/// Cada operación usa el identificador del token (claim <c>sub</c>): nunca recibe el id por la ruta.
/// </summary>
[ApiController]
[Authorize]
[Route("api/autenticacion/perfil")]
public sealed class PerfilController : ControllerBase
{
    private readonly PerfilService _servicio;

    /// <summary>
    /// Inicializa el controlador de perfil.
    /// </summary>
    /// <param name="servicio">Servicio que ejecuta las operaciones del perfil.</param>
    public PerfilController(PerfilService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Obtiene los datos del perfil del usuario autenticado.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El perfil, 401 si el token no identifica a un usuario o 404 si el usuario no existe o está inactivo.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerfilResponse>> Obtener(CancellationToken tokenCancelacion)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var resultado = await _servicio.ObtenerAsync(usuarioId, tokenCancelacion);
        return resultado.Estado switch
        {
            EstadoPerfil.Ok => Ok(ConvertirRespuesta(resultado.Usuario!)),
            _ => NotFound(new { mensaje = "No se encontró el perfil del usuario." })
        };
    }

    /// <summary>
    /// Actualiza los nombres y apellidos del usuario autenticado.
    /// </summary>
    /// <param name="solicitud">Datos personales nuevos.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El perfil actualizado, 401 si el token no identifica a un usuario o 404 si el usuario no existe o está inactivo.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(PerfilResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerfilResponse>> ActualizarDatos(
        ActualizarPerfilRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var resultado = await _servicio.ActualizarDatosAsync(usuarioId, solicitud, tokenCancelacion);
        return resultado.Estado switch
        {
            EstadoPerfil.Ok => Ok(ConvertirRespuesta(resultado.Usuario!)),
            _ => NotFound(new { mensaje = "No se encontró el perfil del usuario." })
        };
    }

    /// <summary>
    /// Cambia la contraseña del usuario autenticado tras verificar la actual.
    /// </summary>
    /// <param name="solicitud">Contraseña actual y nueva.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>204 si se cambió, 400 si la contraseña actual no coincide, 401 si el token no identifica a un usuario o 404 si el usuario no existe o está inactivo.</returns>
    [HttpPut("contrasena")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarContrasena(
        CambiarContrasenaRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        if (!TryObtenerUsuarioId(out var usuarioId))
        {
            return Unauthorized();
        }

        var resultado = await _servicio.CambiarContrasenaAsync(usuarioId, solicitud, tokenCancelacion);
        return resultado.Estado switch
        {
            EstadoPerfil.Ok => NoContent(),
            EstadoPerfil.ContrasenaActualIncorrecta => BadRequest(
                new { mensaje = "La contraseña actual no es correcta." }),
            _ => NotFound(new { mensaje = "No se encontró el perfil del usuario." })
        };
    }

    /// <summary>
    /// Lee el identificador del usuario desde el claim <c>sub</c> del token.
    /// </summary>
    /// <param name="usuarioId">Identificador leído, o <see cref="Guid.Empty"/> si no es válido.</param>
    /// <returns><see langword="true"/> si el claim existe y es un identificador válido.</returns>
    private bool TryObtenerUsuarioId(out Guid usuarioId)
    {
        return Guid.TryParse(User.FindFirst("sub")?.Value, out usuarioId);
    }

    private static PerfilResponse ConvertirRespuesta(Usuario usuario)
    {
        var roles = usuario.Roles
            .Select(asignacion => asignacion.Rol.Nombre)
            .OrderBy(nombre => nombre)
            .ToList();

        return new PerfilResponse(
            usuario.Id,
            usuario.PrimerNombre,
            usuario.SegundoNombre,
            usuario.PrimerApellido,
            usuario.SegundoApellido,
            usuario.CorreoElectronico,
            roles);
    }
}
