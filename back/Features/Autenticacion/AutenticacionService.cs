using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Autenticacion.Contracts;
using Runpd.Api.Features.Autenticacion.Correo;
using Runpd.Api.Features.Usuarios;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Valida las credenciales de inicio de sesión, genera tokens de acceso y gestiona la recuperación de contraseña.
/// </summary>
public sealed class AutenticacionService
{
    private readonly RunpdDbContext _contexto;
    private readonly IPasswordHasher<Usuario> _hasherContrasena;
    private readonly JwtTokenService _generadorToken;
    private readonly GeneradorContrasenaTemporal _generadorContrasenaTemporal;
    private readonly IServicioCorreo _servicioCorreo;
    private readonly ILogger<AutenticacionService> _logger;

    /// <summary>
    /// Inicializa el servicio de autenticación.
    /// </summary>
    /// <param name="contexto">Contexto para consultar usuarios y roles.</param>
    /// <param name="hasherContrasena">Verificador de hashes de contraseñas.</param>
    /// <param name="generadorToken">Generador de tokens JWT firmados.</param>
    /// <param name="generadorContrasenaTemporal">Generador de contraseñas temporales.</param>
    /// <param name="servicioCorreo">Servicio que envía los correos de recuperación.</param>
    /// <param name="logger">Registro de errores del servicio.</param>
    public AutenticacionService(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        JwtTokenService generadorToken,
        GeneradorContrasenaTemporal generadorContrasenaTemporal,
        IServicioCorreo servicioCorreo,
        ILogger<AutenticacionService> logger)
    {
        _contexto = contexto;
        _hasherContrasena = hasherContrasena;
        _generadorToken = generadorToken;
        _generadorContrasenaTemporal = generadorContrasenaTemporal;
        _servicioCorreo = servicioCorreo;
        _logger = logger;
    }

    /// <summary>
    /// Valida las credenciales y obtiene un token para un usuario activo.
    /// </summary>
    /// <param name="solicitud">Correo electrónico y contraseña recibidos.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>Token JWT o <see langword="null"/> si las credenciales no son válidas.</returns>
    public async Task<LoginResponse?> IniciarSesionAsync(
        LoginRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var correoNormalizado = solicitud.CorreoElectronico.Trim().ToLowerInvariant();
        var usuario = await _contexto.Usuarios
            .AsNoTracking()
            .Include(usuarioActual => usuarioActual.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(
                usuarioActual => usuarioActual.CorreoElectronico == correoNormalizado
                    && usuarioActual.Activo,
                tokenCancelacion);

        if (usuario?.Contrasena is null)
        {
            return null;
        }

        var resultadoVerificacion = _hasherContrasena.VerifyHashedPassword(
            usuario,
            usuario.Contrasena,
            solicitud.Contrasena);

        if (resultadoVerificacion == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var (token, expiraUtc) = _generadorToken.Generar(usuario);
        return new LoginResponse(token, "Bearer", expiraUtc, usuario.DebeCambiarContrasena);
    }

    /// <summary>
    /// Genera una contraseña temporal para un usuario activo y se la envía por correo.
    /// No revela si el correo existe: si no hay un usuario activo, termina sin hacer nada.
    /// Si el envío falla, la contraseña actual no cambia; el error se registra y no se expone al llamador.
    /// </summary>
    /// <param name="solicitud">Correo electrónico de la cuenta.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    public async Task SolicitarRecuperacionAsync(
        RecuperarContrasenaRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        // Paso 1: buscar un usuario activo por correo normalizado.
        var correoNormalizado = solicitud.CorreoElectronico.Trim().ToLowerInvariant();
        var usuario = await _contexto.Usuarios.SingleOrDefaultAsync(
            usuarioActual => usuarioActual.CorreoElectronico == correoNormalizado
                && usuarioActual.Activo,
            tokenCancelacion);

        if (usuario is null)
        {
            return;
        }

        // Paso 2: enviar la contraseña temporal. Se envía antes de guardarla para que una falla
        // de correo no deje al usuario con una clave que nunca recibió.
        var contrasenaTemporal = _generadorContrasenaTemporal.Generar();
        try
        {
            await _servicioCorreo.EnviarContrasenaTemporalAsync(
                usuario.CorreoElectronico,
                usuario.PrimerNombre,
                contrasenaTemporal,
                tokenCancelacion);
        }
        catch (Exception excepcion) when (excepcion is not OperationCanceledException)
        {
            // Cualquier fallo del canal SMTP (red, autenticación, protocolo) se registra sin exponer detalles al cliente.
            _logger.LogError(excepcion, "No se pudo enviar el correo de recuperación al usuario {UsuarioId}.", usuario.Id);
            return;
        }

        // Paso 3: guardar el hash y exigir el cambio de contraseña en el siguiente inicio de sesión.
        usuario.EstablecerContrasena(_hasherContrasena.HashPassword(usuario, contrasenaTemporal));
        usuario.EstablecerDebeCambiarContrasena(true);
        await _contexto.SaveChangesAsync(tokenCancelacion);
    }
}
