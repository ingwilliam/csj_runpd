using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Departamentos;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de departamentos al esquema de SQL Server.
/// </summary>
public sealed class MaestraDepartamentoConfiguration : IEntityTypeConfiguration<MaestraDepartamento>
{
    /// <summary>
    /// Define los nombres, restricciones y el índice único del código DIVIPOLA.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraDepartamento> constructor)
    {
        constructor.ToTable("MaestraDepartamentos");

        constructor.HasKey(departamento => departamento.Id);

        constructor.Property(departamento => departamento.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(departamento => departamento.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(departamento => departamento.CodigoDivpola)
            .HasColumnName("CodigoDivpola")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        constructor.Property(departamento => departamento.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(departamento => departamento.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // El código DIVIPOLA identifica al departamento de forma única, igual que el UNIQUE del esquema original.
        constructor.HasIndex(departamento => departamento.CodigoDivpola)
            .IsUnique()
            .HasDatabaseName("UX_MaestraDepartamentos_CodigoDivpola");
    }
}
