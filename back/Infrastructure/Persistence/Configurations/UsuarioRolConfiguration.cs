using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.Roles;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de la asignación de roles a usuarios.
/// </summary>
public sealed class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    /// <summary>
    /// Define la clave compuesta y las relaciones de la tabla puente.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<UsuarioRol> constructor)
    {
        constructor.ToTable("UsuariosRoles");

        constructor.HasKey(asignacion => new { asignacion.UsuarioId, asignacion.RolId });

        constructor.Property(asignacion => asignacion.UsuarioId)
            .HasColumnName("UsuarioId");

        constructor.Property(asignacion => asignacion.RolId)
            .HasColumnName("RolId");

        constructor.HasOne(asignacion => asignacion.Usuario)
            .WithMany(usuario => usuario.Roles)
            .HasForeignKey(asignacion => asignacion.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        constructor.HasOne(asignacion => asignacion.Rol)
            .WithMany(rol => rol.Usuarios)
            .HasForeignKey(asignacion => asignacion.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasIndex(asignacion => asignacion.RolId)
            .HasDatabaseName("IX_UsuariosRoles_RolId");
    }
}
