using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Especialidades;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de sub especialidades al esquema de SQL Server y su relación opcional con especialidades.
/// </summary>
public sealed class MaestraSubEspecialidadConfiguration : IEntityTypeConfiguration<MaestraSubEspecialidad>
{
    /// <summary>
    /// Define los nombres, restricciones y la relación opcional con especialidades.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraSubEspecialidad> constructor)
    {
        constructor.ToTable("MaestraSubEspecialidades");

        constructor.HasKey(subEspecialidad => subEspecialidad.Id);

        constructor.Property(subEspecialidad => subEspecialidad.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(subEspecialidad => subEspecialidad.EspecialidadId)
            .HasColumnName("EspecialidadId");

        constructor.Property(subEspecialidad => subEspecialidad.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        // Descripción opcional sin límite de longitud, equivalente a text NULL del esquema original.
        constructor.Property(subEspecialidad => subEspecialidad.Descripcion)
            .HasColumnName("Descripcion")
            .HasColumnType("nvarchar(max)");

        constructor.Property(subEspecialidad => subEspecialidad.Promiscuo)
            .HasColumnName("Promiscuo")
            .IsRequired();

        // Código opcional y no único: el catálogo original repite códigos entre sub especialidades.
        constructor.Property(subEspecialidad => subEspecialidad.Codigo)
            .HasColumnName("Codigo")
            .HasMaxLength(20)
            .IsUnicode(false);

        constructor.Property(subEspecialidad => subEspecialidad.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(subEspecialidad => subEspecialidad.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Restrict: no se puede borrar una especialidad que tenga sub especialidades asociadas.
        constructor.HasOne<MaestraEspecialidad>()
            .WithMany()
            .HasForeignKey(subEspecialidad => subEspecialidad.EspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
