using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.Autenticacion.Contracts;

namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Expone el inicio de sesión de los usuarios.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/autenticacion")]
public sealed class AutenticacionController : ControllerBase
{
    private readonly AutenticacionService _servicio;

    /// <summary>
    /// Inicializa el controlador de autenticación.
    /// </summary>
    /// <param name="servicio">Servicio que valida credenciales y genera tokens.</param>
    public AutenticacionController(AutenticacionService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Valida las credenciales y devuelve un token JWT.
    /// </summary>
    /// <param name="solicitud">Correo electrónico y contraseña del usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>Token JWT o 401 si las credenciales no son válidas.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> IniciarSesion(
        LoginRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.IniciarSesionAsync(solicitud, tokenCancelacion);
        if (resultado is null)
        {
            return Unauthorized(new { mensaje = "El correo electrónico o la contraseña no son válidos." });
        }

        return Ok(resultado);
    }

    /// <summary>
    /// Envía una contraseña temporal al correo de un usuario activo.
    /// La respuesta es la misma exista o no el correo, para no revelar qué cuentas están registradas.
    /// </summary>
    /// <param name="solicitud">Correo electrónico de la cuenta.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>200 con un mensaje genérico, o 400 si el correo no es válido.</returns>
    [HttpPost("recuperar-contrasena")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecuperarContrasena(
        RecuperarContrasenaRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        await _servicio.SolicitarRecuperacionAsync(solicitud, tokenCancelacion);
        return Ok(new { mensaje = "Si el correo está registrado, recibirás una contraseña temporal en tu bandeja de entrada." });
    }
}
