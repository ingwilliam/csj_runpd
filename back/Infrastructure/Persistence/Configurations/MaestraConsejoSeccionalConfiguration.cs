using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de consejos seccionales al esquema de SQL Server.
/// </summary>
public sealed class MaestraConsejoSeccionalConfiguration : IEntityTypeConfiguration<MaestraConsejoSeccional>
{
    /// <summary>
    /// Define los nombres y restricciones de la tabla de consejos seccionales.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraConsejoSeccional> constructor)
    {
        constructor.ToTable("MaestraConsejosSeccionales");

        constructor.HasKey(consejo => consejo.Id);

        constructor.Property(consejo => consejo.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(consejo => consejo.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(consejo => consejo.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(consejo => consejo.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();
    }
}
