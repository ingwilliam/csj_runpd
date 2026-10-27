using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de usuarios al esquema de SQL Server.
/// </summary>
public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    /// <summary>
    /// Define los nombres y restricciones de la tabla de usuarios.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<Usuario> constructor)
    {
        constructor.ToTable("Usuarios");

        constructor.HasKey(usuario => usuario.Id);

        constructor.Property(usuario => usuario.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(usuario => usuario.PrimerNombre)
            .HasColumnName("PrimerNombre")
            .HasMaxLength(100)
            .IsRequired();

        constructor.Property(usuario => usuario.SegundoNombre)
            .HasColumnName("SegundoNombre")
            .HasMaxLength(100);

        constructor.Property(usuario => usuario.PrimerApellido)
            .HasColumnName("PrimerApellido")
            .HasMaxLength(100)
            .IsRequired();

        constructor.Property(usuario => usuario.SegundoApellido)
            .HasColumnName("SegundoApellido")
            .HasMaxLength(100);

        constructor.Property(usuario => usuario.CorreoElectronico)
            .HasColumnName("CorreoElectronico")
            .HasMaxLength(254)
            .IsRequired();

        constructor.Property(usuario => usuario.Contrasena)
            .HasColumnName("Contrasena")
            .HasMaxLength(200);

        constructor.Property(usuario => usuario.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(usuario => usuario.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        constructor.HasIndex(usuario => usuario.CorreoElectronico)
            .IsUnique()
            .HasDatabaseName("UX_Usuarios_CorreoElectronico");
    }
}
