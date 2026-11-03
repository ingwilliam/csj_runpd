using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;
using Runpd.Api.Features.TablasMaestras.Departamentos;
using Runpd.Api.Features.TablasMaestras.Municipios;
using Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;
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
    /// Obtiene los consejos seccionales del catálogo maestro.
    /// </summary>
    public DbSet<MaestraConsejoSeccional> MaestraConsejosSeccionales => Set<MaestraConsejoSeccional>();

    /// <summary>
    /// Obtiene las direcciones seccionales del catálogo maestro.
    /// </summary>
    public DbSet<MaestraDireccionSeccional> MaestraDireccionesSeccionales => Set<MaestraDireccionSeccional>();

    /// <summary>
    /// Obtiene los departamentos del catálogo maestro.
    /// </summary>
    public DbSet<MaestraDepartamento> MaestraDepartamentos => Set<MaestraDepartamento>();

    /// <summary>
    /// Obtiene los municipios del catálogo maestro.
    /// </summary>
    public DbSet<MaestraMunicipio> MaestraMunicipios => Set<MaestraMunicipio>();

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
