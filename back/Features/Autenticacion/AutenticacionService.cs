using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Autenticacion.Contracts;
using Runpd.Api.Features.Usuarios;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Valida las credenciales de inicio de sesión y genera tokens de acceso.
/// </summary>
public sealed class AutenticacionService
{
    private readonly RunpdDbContext _contexto;
    private readonly IPasswordHasher<Usuario> _hasherContrasena;
    private readonly JwtTokenService _generadorToken;

    /// <summary>
    /// Inicializa el servicio de autenticación.
    /// </summary>
    /// <param name="contexto">Contexto para consultar usuarios y roles.</param>
    /// <param name="hasherContrasena">Verificador de hashes de contraseñas.</param>
    /// <param name="generadorToken">Generador de tokens JWT firmados.</param>
    public AutenticacionService(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        JwtTokenService generadorToken)
    {
        _contexto = contexto;
        _hasherContrasena = hasherContrasena;
        _generadorToken = generadorToken;
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
        return new LoginResponse(token, "Bearer", expiraUtc);
    }
}
