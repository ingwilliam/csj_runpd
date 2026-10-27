using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.Roles;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de roles al esquema de SQL Server.
/// </summary>
public sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    /// <summary>
    /// Define los nombres, restricciones y datos iniciales de los roles.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<Rol> constructor)
    {
        constructor.ToTable("Roles");

        constructor.HasKey(rol => rol.Id);

        constructor.Property(rol => rol.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(rol => rol.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(50)
            .IsRequired();

        constructor.Property(rol => rol.Descripcion)
            .HasColumnName("Descripcion")
            .HasMaxLength(200);

        constructor.Property(rol => rol.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(rol => rol.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        constructor.HasIndex(rol => rol.Nombre)
            .IsUnique()
            .HasDatabaseName("UX_Roles_Nombre");
    }
}
