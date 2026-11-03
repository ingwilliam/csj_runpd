using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;
using Runpd.Api.Features.TablasMaestras.Departamentos;
using Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;
using Runpd.Api.Features.TablasMaestras.Municipios;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de municipios al esquema de SQL Server, sus claves foráneas y el índice único del código DIVIPOLA.
/// </summary>
public sealed class MaestraMunicipioConfiguration : IEntityTypeConfiguration<MaestraMunicipio>
{
    /// <summary>
    /// Define las columnas, restricciones y relaciones de la tabla de municipios.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraMunicipio> constructor)
    {
        constructor.ToTable("MaestraMunicipios");

        constructor.HasKey(municipio => municipio.Id);

        constructor.Property(municipio => municipio.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(municipio => municipio.Tipo)
            .HasColumnName("Tipo")
            .HasMaxLength(150)
            .IsUnicode(false);

        constructor.Property(municipio => municipio.Pdet)
            .HasColumnName("Pdet")
            .IsRequired();

        constructor.Property(municipio => municipio.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(municipio => municipio.CodigoDivpola)
            .HasColumnName("CodigoDivpola")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        constructor.Property(municipio => municipio.DepartamentoId)
            .HasColumnName("DepartamentoId")
            .IsRequired();

        constructor.Property(municipio => municipio.ConsejoSeccionalId)
            .HasColumnName("ConsejoSeccionalId");

        constructor.Property(municipio => municipio.DireccionSeccionalId)
            .HasColumnName("DireccionSeccionalId");

        constructor.Property(municipio => municipio.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(municipio => municipio.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // El código DIVIPOLA identifica al municipio de forma única, igual que el UNIQUE del esquema original.
        constructor.HasIndex(municipio => municipio.CodigoDivpola)
            .IsUnique()
            .HasDatabaseName("UX_MaestraMunicipios_CodigoDivpola");

        // Restrict: no se puede borrar un departamento que tenga municipios.
        constructor.HasOne<MaestraDepartamento>()
            .WithMany()
            .HasForeignKey(municipio => municipio.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        // SetNull: si desaparece el consejo o la dirección, el municipio queda sin asignación.
        constructor.HasOne<MaestraConsejoSeccional>()
            .WithMany()
            .HasForeignKey(municipio => municipio.ConsejoSeccionalId)
            .OnDelete(DeleteBehavior.SetNull);

        constructor.HasOne<MaestraDireccionSeccional>()
            .WithMany()
            .HasForeignKey(municipio => municipio.DireccionSeccionalId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
