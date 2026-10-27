using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Infrastructure.Persistence;

/// <summary>
/// Proporciona acceso de Entity Framework Core a la base de datos Runpd.
/// </summary>
/// <param name="options">Opciones de configuración del contexto.</param>
public sealed class RunpdDbContext(DbContextOptions<RunpdDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Obtiene los usuarios almacenados en la base de datos.
    /// </summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>
    /// Obtiene los roles almacenados en la base de datos.
    /// </summary>
    public DbSet<Rol> Roles => Set<Rol>();

    /// <summary>
    /// Obtiene las asignaciones de roles a usuarios.
    /// </summary>
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();

    /// <summary>
    /// Aplica las configuraciones explícitas de las entidades.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RunpdDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
