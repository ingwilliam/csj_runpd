using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Perfil.Contracts;
using Runpd.Api.Features.Usuarios;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Perfil;

/// <summary>
/// Ejecuta las operaciones sobre el perfil del propio usuario autenticado.
/// </summary>
public sealed class PerfilService
{
    private readonly RunpdDbContext _contexto;
    private readonly IPasswordHasher<Usuario> _hasherContrasena;

    /// <summary>
    /// Inicializa el servicio de perfil.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    /// <param name="hasherContrasena">Calculador y verificador de hashes de contraseñas.</param>
    public PerfilService(RunpdDbContext contexto, IPasswordHasher<Usuario> hasherContrasena)
    {
        _contexto = contexto;
        _hasherContrasena = hasherContrasena;
    }

    /// <summary>
    /// Obtiene el perfil del usuario indicado, con sus roles.
    /// </summary>
    /// <param name="id">Identificador del usuario autenticado.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario o <see cref="EstadoPerfil.UsuarioNoEncontrado"/>.</returns>
    public async Task<PerfilResultado> ObtenerAsync(Guid id, CancellationToken tokenCancelacion)
    {
        var usuario = await BuscarUsuarioActivoAsync(id, tokenCancelacion);
        if (usuario is null)
        {
            return new PerfilResultado(EstadoPerfil.UsuarioNoEncontrado, null);
        }

        return new PerfilResultado(EstadoPerfil.Ok, usuario);
    }

    /// <summary>
    /// Actualiza los datos personales del usuario. El correo y los roles no se modifican aquí.
    /// </summary>
    /// <param name="id">Identificador del usuario autenticado.</param>
    /// <param name="solicitud">Nombres y apellidos nuevos.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario actualizado o <see cref="EstadoPerfil.UsuarioNoEncontrado"/>.</returns>
    public async Task<PerfilResultado> ActualizarDatosAsync(
        Guid id,
        ActualizarPerfilRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var usuario = await BuscarUsuarioActivoAsync(id, tokenCancelacion);
        if (usuario is null)
        {
            return new PerfilResultado(EstadoPerfil.UsuarioNoEncontrado, null);
        }

        // El correo se conserva: es el identificador de inicio de sesión y no se edita desde el perfil.
        usuario.Actualizar(
            solicitud.PrimerNombre.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoNombre),
            solicitud.PrimerApellido.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoApellido),
            usuario.CorreoElectronico);

        await _contexto.SaveChangesAsync(tokenCancelacion);
        return new PerfilResultado(EstadoPerfil.Ok, usuario);
    }

    /// <summary>
    /// Cambia la contraseña del usuario tras verificar la contraseña actual.
    /// Las sesiones ya emitidas siguen válidas hasta que venza su token.
    /// </summary>
    /// <param name="id">Identificador del usuario autenticado.</param>
    /// <param name="solicitud">Contraseña actual y nueva.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>
    /// El resultado de la operación: <see cref="EstadoPerfil.ContrasenaActualIncorrecta"/> si la
    /// contraseña actual no coincide, o <see cref="EstadoPerfil.UsuarioNoEncontrado"/>.
    /// </returns>
    public async Task<PerfilResultado> CambiarContrasenaAsync(
        Guid id,
        CambiarContrasenaRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var usuario = await BuscarUsuarioActivoAsync(id, tokenCancelacion);
        if (usuario is null)
        {
            return new PerfilResultado(EstadoPerfil.UsuarioNoEncontrado, null);
        }

        // Paso 1: verificar la contraseña actual. Sin hash registrado no hay forma de comprobarla.
        if (usuario.Contrasena is null
            || _hasherContrasena.VerifyHashedPassword(usuario, usuario.Contrasena, solicitud.ContrasenaActual)
                == PasswordVerificationResult.Failed)
        {
            return new PerfilResultado(EstadoPerfil.ContrasenaActualIncorrecta, null);
        }

        // Paso 2: guardar el hash de la nueva contraseña.
        usuario.EstablecerContrasena(_hasherContrasena.HashPassword(usuario, solicitud.ContrasenaNueva));
        // Paso 3: la contrase\u00f1a ya es personal, as\u00ed que deja de exigirse el cambio.
        usuario.EstablecerDebeCambiarContrasena(false);
        await _contexto.SaveChangesAsync(tokenCancelacion);
        return new PerfilResultado(EstadoPerfil.Ok, usuario);
    }

    /// <summary>
    /// Busca un usuario activo por su identificador, incluyendo sus roles.
    /// Un usuario desactivado se trata como inexistente aunque su token siga vigente.
    /// </summary>
    private async Task<Usuario?> BuscarUsuarioActivoAsync(Guid id, CancellationToken tokenCancelacion)
    {
        return await _contexto.Usuarios
            .Include(usuario => usuario.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(
                usuario => usuario.Id == id && usuario.Activo,
                tokenCancelacion);
    }

    private static string? NormalizarNombreOpcional(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return null;
        }

        return nombre.Trim();
    }
}
