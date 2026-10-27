using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Roles.Contracts;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Roles;

/// <summary>
/// Ejecuta las operaciones de administración de roles.
/// </summary>
public sealed class RolesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de roles.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public RolesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene todos los roles, incluidos los desactivados.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de roles ordenada por nombre.</returns>
    public async Task<List<Rol>> ObtenerTodosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.Roles
            .AsNoTracking()
            .OrderBy(rol => rol.Nombre)
            .ToListAsync(tokenCancelacion);
    }

    /// <summary>
    /// Crea un rol nuevo.
    /// </summary>
    /// <param name="solicitud">Datos validados del rol.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El rol creado o <see langword="null"/> si el nombre ya existe.</returns>
    public async Task<Rol?> CrearAsync(
        CreateRolRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var nombre = solicitud.Nombre.Trim();

        if (await _contexto.Roles.AnyAsync(rol => rol.Nombre == nombre, tokenCancelacion))
        {
            return null;
        }

        var rol = new Rol(nombre, NormalizarDescripcion(solicitud.Descripcion));
        _contexto.Roles.Add(rol);

        try
        {
            await _contexto.SaveChangesAsync(tokenCancelacion);
            return rol;
        }
        catch (DbUpdateException excepcion) when (EsViolacionDeIndiceUnico(excepcion))
        {
            return null;
        }
    }

    /// <summary>
    /// Cambia el estado activo de un rol sin borrar sus datos.
    /// </summary>
    /// <param name="id">Identificador del rol.</param>
    /// <param name="activo">Nuevo estado activo.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el rol actualizado cuando la operación es válida.</returns>
    public async Task<CambiarActivoRolResultado> CambiarActivoAsync(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var rol = await _contexto.Roles
            .SingleOrDefaultAsync(rolActual => rolActual.Id == id, tokenCancelacion);

        if (rol is null)
        {
            return new CambiarActivoRolResultado(EstadoCambiarActivoRol.NoEncontrado, null);
        }

        if (!activo)
        {
            // Los roles del sistema y los que tienen usuarios asignados no pueden desactivarse.
            if (EsRolDelSistema(rol) || await TieneUsuariosAsignadosAsync(id, tokenCancelacion))
            {
                return new CambiarActivoRolResultado(EstadoCambiarActivoRol.EnUso, null);
            }
        }

        rol.EstablecerActivo(activo);
        await _contexto.SaveChangesAsync(tokenCancelacion);

        return new CambiarActivoRolResultado(EstadoCambiarActivoRol.Ok, rol);
    }

    private async Task<bool> TieneUsuariosAsignadosAsync(
        Guid id,
        CancellationToken tokenCancelacion)
    {
        return await _contexto.UsuariosRoles
            .AnyAsync(asignacion => asignacion.RolId == id, tokenCancelacion);
    }

    private static bool EsRolDelSistema(Rol rol)
    {
        return RolesConocidos.RolesDelSistema.Contains(rol.Nombre);
    }

    private static string? NormalizarDescripcion(string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            return null;
        }

        return descripcion.Trim();
    }

    private static bool EsViolacionDeIndiceUnico(DbUpdateException excepcion)
    {
        return excepcion.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
