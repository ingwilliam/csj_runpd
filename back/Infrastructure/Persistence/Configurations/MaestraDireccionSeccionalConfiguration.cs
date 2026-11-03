using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de direcciones seccionales al esquema de SQL Server.
/// </summary>
public sealed class MaestraDireccionSeccionalConfiguration : IEntityTypeConfiguration<MaestraDireccionSeccional>
{
    /// <summary>
    /// Define los nombres y restricciones de la tabla de direcciones seccionales.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraDireccionSeccional> constructor)
    {
        constructor.ToTable("MaestraDireccionesSeccionales");

        constructor.HasKey(direccion => direccion.Id);

        constructor.Property(direccion => direccion.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(direccion => direccion.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(direccion => direccion.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(direccion => direccion.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();
    }
}
