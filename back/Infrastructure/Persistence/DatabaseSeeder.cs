using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Infrastructure.Persistence;

/// <summary>
/// Garantiza los datos mínimos de arranque: roles del sistema y un administrador activo.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Siembra los roles del sistema y garantiza que exista un administrador activo.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    /// <param name="hasherContrasena">Calculador del hash para el administrador inicial.</param>
    /// <param name="configuracion">Configuración con las variables del administrador inicial.</param>
    /// <param name="logger">Registro de eventos del arranque.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    public static async Task SeedAsync(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        IConfiguration configuracion,
        ILogger logger,
        CancellationToken tokenCancelacion = default)
    {
        await SembrarRolesAsync(contexto, logger, tokenCancelacion);
        await GarantizarAdministradorAsync(
            contexto,
            hasherContrasena,
            configuracion,
            logger,
            tokenCancelacion);
    }

    private static async Task SembrarRolesAsync(
        RunpdDbContext contexto,
        ILogger logger,
        CancellationToken tokenCancelacion)
    {
        var rolesExistentes = await contexto.Roles.ToListAsync(tokenCancelacion);
        var creados = 0;
        var reactivados = 0;

        foreach (var rolEsperado in RolesConocidos.Catalogo)
        {
            var rol = rolesExistentes.SingleOrDefault(rolActual =>
                string.Equals(
                    rolActual.Nombre,
                    rolEsperado.Nombre,
                    StringComparison.OrdinalIgnoreCase));

            if (rol is null)
            {
                contexto.Roles.Add(new Rol(rolEsperado.Nombre, rolEsperado.Descripcion));
                creados++;
                continue;
            }

            if (!rol.Activo)
            {
                // Un cambio manual no debe dejar inactivo un rol del sistema.
                rol.EstablecerActivo(true);
                reactivados++;
            }
        }

        if (creados == 0 && reactivados == 0)
        {
            return;
        }

        await contexto.SaveChangesAsync(tokenCancelacion);
        logger.LogInformation(
            "Roles del sistema verificados: {Creados} creados y {Reactivados} reactivados.",
            creados,
            reactivados);
    }

    private static async Task GarantizarAdministradorAsync(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        IConfiguration configuracion,
        ILogger logger,
        CancellationToken tokenCancelacion)
    {
        var rolAdministrador = await contexto.Roles.SingleOrDefaultAsync(
            rol => rol.Nombre == RolesConocidos.AdministradorDelSistema,
            tokenCancelacion);

        if (rolAdministrador is null)
        {
            logger.LogWarning(
                "No se encontró el rol {Rol} después de la siembra.",
                RolesConocidos.AdministradorDelSistema);
            return;
        }

        var correoAdministrador = configuracion["ADMIN_EMAIL"]?.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(correoAdministrador))
        {
            var usuario = await contexto.Usuarios
                .Include(usuarioActual => usuarioActual.Roles)
                .SingleOrDefaultAsync(
                    usuarioActual => usuarioActual.CorreoElectronico == correoAdministrador,
                    tokenCancelacion);

            if (usuario is not null)
            {
                // El usuario configurado existe: se le garantiza el rol, el estado activo y la contraseña inicial.
                await AsegurarAdministradorExistenteAsync(
                    contexto,
                    hasherContrasena,
                    configuracion,
                    rolAdministrador,
                    usuario,
                    logger,
                    tokenCancelacion);
                return;
            }
        }

        // Sin usuario configurado existente: solo se interviene si no hay ningún administrador activo.
        var existeAdministradorActivo = await contexto.UsuariosRoles
            .AnyAsync(
                asignacion => asignacion.Rol.Nombre == RolesConocidos.AdministradorDelSistema
                    && asignacion.Usuario.Activo,
                tokenCancelacion);

        if (existeAdministradorActivo)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(correoAdministrador))
        {
            logger.LogWarning(
                "No hay un administrador del sistema activo. Configura ADMIN_EMAIL (y ADMIN_PASSWORD si el usuario no existe) para restaurarlo.");
            return;
        }

        var usuarioNuevo = CrearAdministrador(
            contexto,
            hasherContrasena,
            configuracion,
            correoAdministrador,
            logger);

        if (usuarioNuevo is null)
        {
            return;
        }

        usuarioNuevo.Roles.Add(new UsuarioRol(usuarioNuevo, rolAdministrador));
        await contexto.SaveChangesAsync(tokenCancelacion);
        logger.LogInformation(
            "Rol {Rol} asignado al usuario administrador configurado.",
            RolesConocidos.AdministradorDelSistema);
    }

    private static async Task AsegurarAdministradorExistenteAsync(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        IConfiguration configuracion,
        Rol rolAdministrador,
        Usuario usuario,
        ILogger logger,
        CancellationToken tokenCancelacion)
    {
        var cambio = false;

        if (!usuario.Activo)
        {
            usuario.EstablecerActivo(true);
            cambio = true;
            logger.LogInformation("El usuario administrador configurado fue reactivado.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            // La contraseña solo se asigna si el administrador no tiene una; nunca se sobrescribe.
            var contrasena = configuracion["ADMIN_PASSWORD"];
            if (!string.IsNullOrWhiteSpace(contrasena) && contrasena.Length >= 8)
            {
                usuario.EstablecerContrasena(hasherContrasena.HashPassword(usuario, contrasena));
                cambio = true;
                logger.LogInformation("Se asignó la contraseña inicial al usuario administrador configurado.");
            }
        }

        if (!usuario.Roles.Any(asignacion => asignacion.RolId == rolAdministrador.Id))
        {
            usuario.Roles.Add(new UsuarioRol(usuario, rolAdministrador));
            cambio = true;
            logger.LogInformation(
                "Rol {Rol} asignado al usuario administrador configurado.",
                RolesConocidos.AdministradorDelSistema);
        }

        if (cambio)
        {
            await contexto.SaveChangesAsync(tokenCancelacion);
        }
    }

    private static Usuario? CrearAdministrador(
        RunpdDbContext contexto,
        IPasswordHasher<Usuario> hasherContrasena,
        IConfiguration configuracion,
        string correoAdministrador,
        ILogger logger)
    {
        var contrasena = configuracion["ADMIN_PASSWORD"];
        if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8)
        {
            logger.LogWarning(
                "El usuario ADMIN_EMAIL no existe y ADMIN_PASSWORD no está configurada o tiene menos de 8 caracteres. No se creó el administrador.");
            return null;
        }

        var usuario = new Usuario(
            ObtenerNombreOpcional(configuracion, "ADMIN_PRIMER_NOMBRE") ?? "Administrador",
            ObtenerNombreOpcional(configuracion, "ADMIN_SEGUNDO_NOMBRE"),
            ObtenerNombreOpcional(configuracion, "ADMIN_PRIMER_APELLIDO") ?? "Sistema",
            ObtenerNombreOpcional(configuracion, "ADMIN_SEGUNDO_APELLIDO"),
            correoAdministrador);

        usuario.EstablecerContrasena(hasherContrasena.HashPassword(usuario, contrasena));
        contexto.Usuarios.Add(usuario);

        logger.LogInformation("Se creó el usuario administrador inicial configurado.");
        return usuario;
    }

    private static string? ObtenerNombreOpcional(IConfiguration configuracion, string clave)
    {
        var nombre = configuracion[clave]?.Trim();
        return string.IsNullOrWhiteSpace(nombre) ? null : nombre;
    }
}
