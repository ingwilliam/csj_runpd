using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.Usuarios.Contracts;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Ejecuta las operaciones de administración de usuarios.
/// </summary>
public sealed class UsuariosService
{
    private const string CollationBusqueda = "Latin1_General_CI_AI";

    private readonly RunpdDbContext _contexto;
    private readonly IPasswordHasher<Usuario> _hasherContrasena;

    /// <summary>
    /// Inicializa el servicio de usuarios.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    /// <param name="hasherContrasena">Calculador del hash de contraseñas.</param>
    public UsuariosService(RunpdDbContext contexto, IPasswordHasher<Usuario> hasherContrasena)
    {
        _contexto = contexto;
        _hasherContrasena = hasherContrasena;
    }

    /// <summary>
    /// Obtiene una página de usuarios con sus roles, aplicando búsqueda y filtro por estado.
    /// </summary>
    /// <param name="consulta">Filtros y paginación solicitados.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La página de usuarios y los totales del listado.</returns>
    public async Task<UsuariosPaginadosResultado> ObtenerPaginadosAsync(
        ListarUsuariosQuery consulta,
        CancellationToken tokenCancelacion)
    {
        var consultaUsuarios = _contexto.Usuarios.AsNoTracking();

        // Paso 1: filtrar por estado activo o inactivo.
        if (consulta.Estado == EstadoUsuarioFiltro.Activos)
        {
            consultaUsuarios = consultaUsuarios.Where(usuario => usuario.Activo);
        }
        else if (consulta.Estado == EstadoUsuarioFiltro.Inactivos)
        {
            consultaUsuarios = consultaUsuarios.Where(usuario => !usuario.Activo);
        }

        // Paso 2: cada palabra debe coincidir con algún dato del usuario.
        // La colación mantiene la búsqueda sin distinguir mayúsculas ni tildes.
        var terminos = (consulta.Busqueda ?? string.Empty)
            .Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var termino in terminos)
        {
            consultaUsuarios = consultaUsuarios.Where(usuario =>
                EF.Functions.Collate(usuario.PrimerNombre, CollationBusqueda)
                    .Contains(termino)
                || (usuario.SegundoNombre != null
                    && EF.Functions.Collate(usuario.SegundoNombre, CollationBusqueda)
                        .Contains(termino))
                || EF.Functions.Collate(usuario.PrimerApellido, CollationBusqueda)
                    .Contains(termino)
                || (usuario.SegundoApellido != null
                    && EF.Functions.Collate(usuario.SegundoApellido, CollationBusqueda)
                        .Contains(termino))
                || EF.Functions.Collate(usuario.CorreoElectronico, CollationBusqueda)
                    .Contains(termino)
                || usuario.Roles.Any(asignacion =>
                    EF.Functions.Collate(asignacion.Rol.Nombre, CollationBusqueda)
                        .Contains(termino)));
        }

        // Paso 3: contar el resultado filtrado y resumir los totales globales.
        var total = await consultaUsuarios.CountAsync(tokenCancelacion);
        var resumen = await _contexto.Usuarios
            .GroupBy(_ => 1)
            .Select(grupo => new
            {
                Total = grupo.Count(),
                Activos = grupo.Count(usuario => usuario.Activo)
            })
            .SingleOrDefaultAsync(tokenCancelacion);

        var totalUsuarios = resumen?.Total ?? 0;
        var totalActivos = resumen?.Activos ?? 0;
        var totalPaginas = total == 0
            ? 0
            : (int)Math.Ceiling(total / (double)consulta.Tamano);

        // Paso 4: obtener la página solicitada, de la más reciente a la más antigua.
        var usuarios = await consultaUsuarios
            .Include(usuario => usuario.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .OrderByDescending(usuario => usuario.FechaCreacionUtc)
            .ThenBy(usuario => usuario.Id)
            .Skip((consulta.Pagina - 1) * consulta.Tamano)
            .Take(consulta.Tamano)
            .AsSplitQuery()
            .ToListAsync(tokenCancelacion);

        return new UsuariosPaginadosResultado(
            usuarios,
            consulta.Pagina,
            consulta.Tamano,
            total,
            totalPaginas,
            totalUsuarios,
            totalActivos,
            totalUsuarios - totalActivos);
    }

    /// <summary>
    /// Busca un usuario por su identificador, incluyendo sus roles.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario encontrado o <see langword="null"/>.</returns>
    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken tokenCancelacion)
    {
        return await _contexto.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(usuario => usuario.Id == id, tokenCancelacion);
    }

    /// <summary>
    /// Crea un usuario nuevo, con o sin roles según la solicitud.
    /// </summary>
    /// <param name="solicitud">Datos validados para crear el usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario creado o el error de validación.</returns>
    public async Task<CrearUsuarioResultado> CrearAsync(
        CreateUsuarioRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var idsRoles = ObtenerIdsRoles(solicitud.Roles);

        // Valida los roles antes de insertar el usuario para no dejar datos parciales.
        var rolesSolicitados = await ObtenerRolesActivosAsync(idsRoles, tokenCancelacion);
        if (rolesSolicitados is null)
        {
            return new CrearUsuarioResultado(EstadoCrearUsuario.RolInvalido, null);
        }

        var usuario = new Usuario(
            solicitud.PrimerNombre.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoNombre),
            solicitud.PrimerApellido.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoApellido),
            NormalizarCorreoElectronico(solicitud.CorreoElectronico));

        usuario.EstablecerContrasena(
            _hasherContrasena.HashPassword(usuario, solicitud.Contrasena));

        foreach (var rol in rolesSolicitados)
        {
            usuario.Roles.Add(new UsuarioRol(usuario, rol));
        }

        _contexto.Usuarios.Add(usuario);

        try
        {
            await _contexto.SaveChangesAsync(tokenCancelacion);
            return new CrearUsuarioResultado(EstadoCrearUsuario.Ok, usuario);
        }
        catch (DbUpdateException excepcion) when (EsViolacionDeIndiceUnico(excepcion))
        {
            return new CrearUsuarioResultado(EstadoCrearUsuario.CorreoDuplicado, null);
        }
    }

    /// <summary>
    /// Actualiza los datos de un usuario y, opcionalmente, reemplaza sus roles.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="solicitud">Nuevos datos del usuario; los roles solo cambian si se envían.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario actualizado o el error de validación.</returns>
    public async Task<ActualizarUsuarioResultado> ActualizarAsync(
        Guid id,
        UpdateUsuarioRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var usuario = await _contexto.Usuarios
            .Include(usuarioActual => usuarioActual.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(
                usuarioActual => usuarioActual.Id == id,
                tokenCancelacion);

        if (usuario is null)
        {
            return new ActualizarUsuarioResultado(EstadoActualizarUsuario.UsuarioNoEncontrado, null);
        }

        usuario.Actualizar(
            solicitud.PrimerNombre.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoNombre),
            solicitud.PrimerApellido.Trim(),
            NormalizarNombreOpcional(solicitud.SegundoApellido),
            NormalizarCorreoElectronico(solicitud.CorreoElectronico));

        // Los roles solo se reemplazan cuando la solicitud los incluye.
        if (solicitud.Roles is not null)
        {
            var rolesSolicitados = await ObtenerRolesActivosAsync(
                ObtenerIdsRoles(solicitud.Roles),
                tokenCancelacion);

            if (rolesSolicitados is null)
            {
                return new ActualizarUsuarioResultado(EstadoActualizarUsuario.RolInvalido, null);
            }

            if (PierdeRolAdministrador(usuario, rolesSolicitados))
            {
                var existeOtroAdministrador = await ExisteOtroAdministradorActivoAsync(
                    usuario.Id,
                    tokenCancelacion);

                if (!existeOtroAdministrador)
                {
                    return new ActualizarUsuarioResultado(
                        EstadoActualizarUsuario.SinAdministrador,
                        null);
                }
            }

            ReemplazarRoles(usuario, rolesSolicitados);
        }

        try
        {
            await _contexto.SaveChangesAsync(tokenCancelacion);
            return new ActualizarUsuarioResultado(EstadoActualizarUsuario.Ok, usuario);
        }
        catch (DbUpdateException excepcion) when (EsViolacionDeIndiceUnico(excepcion))
        {
            return new ActualizarUsuarioResultado(EstadoActualizarUsuario.CorreoDuplicado, null);
        }
    }

    /// <summary>
    /// Cambia el estado activo de un usuario sin borrar sus datos.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="activo">Nuevo estado activo.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario actualizado cuando la operación es válida.</returns>
    public async Task<CambiarActivoResultado> CambiarActivoAsync(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var usuario = await _contexto.Usuarios
            .Include(usuarioActual => usuarioActual.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(
                usuarioActual => usuarioActual.Id == id,
                tokenCancelacion);

        if (usuario is null)
        {
            return new CambiarActivoResultado(EstadoCambiarActivo.UsuarioNoEncontrado, null);
        }

        if (!activo && usuario.Activo && TieneRol(usuario, RolesConocidos.AdministradorDelSistema))
        {
            var existeOtroAdministrador = await ExisteOtroAdministradorActivoAsync(
                usuario.Id,
                tokenCancelacion);

            if (!existeOtroAdministrador)
            {
                return new CambiarActivoResultado(EstadoCambiarActivo.SinAdministrador, null);
            }
        }

        usuario.EstablecerActivo(activo);
        await _contexto.SaveChangesAsync(tokenCancelacion);

        return new CambiarActivoResultado(EstadoCambiarActivo.Ok, usuario);
    }

    /// <summary>
    /// Reemplaza los roles activos asignados a un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="roles">Identificadores de los roles que debe tener el usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el usuario actualizado cuando la operación es válida.</returns>
    public async Task<CambiarRolesResultado> ActualizarRolesAsync(
        Guid id,
        IReadOnlyCollection<Guid> roles,
        CancellationToken tokenCancelacion)
    {
        var usuario = await _contexto.Usuarios
            .Include(usuarioActual => usuarioActual.Roles)
            .ThenInclude(asignacion => asignacion.Rol)
            .SingleOrDefaultAsync(
                usuarioActual => usuarioActual.Id == id,
                tokenCancelacion);

        if (usuario is null)
        {
            return new CambiarRolesResultado(EstadoCambiarRoles.UsuarioNoEncontrado, null);
        }

        var rolesSolicitados = await ObtenerRolesActivosAsync(
            ObtenerIdsRoles(roles),
            tokenCancelacion);

        if (rolesSolicitados is null)
        {
            return new CambiarRolesResultado(EstadoCambiarRoles.RolInvalido, null);
        }

        if (PierdeRolAdministrador(usuario, rolesSolicitados))
        {
            var existeOtroAdministrador = await ExisteOtroAdministradorActivoAsync(
                usuario.Id,
                tokenCancelacion);

            if (!existeOtroAdministrador)
            {
                return new CambiarRolesResultado(EstadoCambiarRoles.SinAdministrador, null);
            }
        }

        ReemplazarRoles(usuario, rolesSolicitados);
        await _contexto.SaveChangesAsync(tokenCancelacion);

        return new CambiarRolesResultado(EstadoCambiarRoles.Ok, usuario);
    }

    /// <summary>
    /// Deduplica los identificadores de roles de una solicitud.
    /// </summary>
    /// <param name="roles">Roles solicitados; puede ser <see langword="null"/>.</param>
    /// <returns>Los identificadores sin repeticiones.</returns>
    private static Guid[] ObtenerIdsRoles(IReadOnlyCollection<Guid>? roles)
    {
        return (roles ?? Array.Empty<Guid>()).Distinct().ToArray();
    }

    /// <summary>
    /// Carga los roles activos correspondientes a los identificadores solicitados.
    /// </summary>
    /// <param name="idsRoles">Identificadores deduplicados.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>Los roles encontrados o <see langword="null"/> si alguno no existe o está inactivo.</returns>
    private async Task<List<Rol>?> ObtenerRolesActivosAsync(
        Guid[] idsRoles,
        CancellationToken tokenCancelacion)
    {
        if (idsRoles.Length == 0)
        {
            return new List<Rol>();
        }

        var roles = await _contexto.Roles
            .Where(rol => idsRoles.Contains(rol.Id) && rol.Activo)
            .ToListAsync(tokenCancelacion);

        return roles.Count == idsRoles.Length ? roles : null;
    }

    /// <summary>
    /// Determina si un usuario activo con rol de administrador del sistema
    /// no lo tendría entre los roles solicitados.
    /// </summary>
    /// <param name="usuario">Usuario que se va a modificar.</param>
    /// <param name="rolesSolicitados">Roles activos que tendrá el usuario.</param>
    /// <returns><see langword="true"/> si perdería el rol de administrador del sistema.</returns>
    private static bool PierdeRolAdministrador(Usuario usuario, List<Rol> rolesSolicitados)
    {
        return usuario.Activo
            && TieneRol(usuario, RolesConocidos.AdministradorDelSistema)
            && !rolesSolicitados.Any(rol => EsRolAdministrador(rol.Nombre));
    }

    /// <summary>
    /// Reemplaza las asignaciones de roles del usuario con los roles solicitados.
    /// </summary>
    /// <param name="usuario">Usuario que se va a modificar.</param>
    /// <param name="rolesSolicitados">Roles activos validados que tendrá el usuario.</param>
    private void ReemplazarRoles(Usuario usuario, List<Rol> rolesSolicitados)
    {
        // Elimina las asignaciones que ya no se solicitan.
        var idsSolicitados = rolesSolicitados.Select(rol => rol.Id).ToHashSet();
        foreach (var asignacion in usuario.Roles
                     .Where(asignacion => !idsSolicitados.Contains(asignacion.RolId))
                     .ToList())
        {
            _contexto.UsuariosRoles.Remove(asignacion);
            usuario.Roles.Remove(asignacion);
        }

        // Agrega únicamente las asignaciones nuevas.
        var idsActuales = usuario.Roles.Select(asignacion => asignacion.RolId).ToHashSet();
        foreach (var rol in rolesSolicitados.Where(rol => !idsActuales.Contains(rol.Id)))
        {
            usuario.Roles.Add(new UsuarioRol(usuario, rol));
        }
    }

    private async Task<bool> ExisteOtroAdministradorActivoAsync(
        Guid idUsuarioExcluido,
        CancellationToken tokenCancelacion)
    {
        return await _contexto.UsuariosRoles
            .AnyAsync(
                asignacion => asignacion.Rol.Nombre == RolesConocidos.AdministradorDelSistema
                    && asignacion.UsuarioId != idUsuarioExcluido
                    && asignacion.Usuario.Activo,
                tokenCancelacion);
    }

    private static bool TieneRol(Usuario usuario, string nombreRol)
    {
        return usuario.Roles.Any(asignacion =>
            string.Equals(asignacion.Rol.Nombre, nombreRol, StringComparison.OrdinalIgnoreCase));
    }

    private static bool EsRolAdministrador(string nombreRol)
    {
        return string.Equals(
            nombreRol,
            RolesConocidos.AdministradorDelSistema,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizarNombreOpcional(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return null;
        }

        return nombre.Trim();
    }

    private static string NormalizarCorreoElectronico(string correoElectronico)
    {
        return correoElectronico.Trim().ToLowerInvariant();
    }

    private static bool EsViolacionDeIndiceUnico(DbUpdateException excepcion)
    {
        return excepcion.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
