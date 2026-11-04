using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Especialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de especialidades al esquema de SQL Server.
/// </summary>
public sealed class MaestraEspecialidadConfiguration : IEntityTypeConfiguration<MaestraEspecialidad>
{
    /// <summary>
    /// Define los nombres y restricciones de la tabla de especialidades.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraEspecialidad> constructor)
    {
        constructor.ToTable("MaestraEspecialidades");

        constructor.HasKey(especialidad => especialidad.Id);

        constructor.Property(especialidad => especialidad.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(especialidad => especialidad.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        // Descripción opcional sin límite de longitud, equivalente a text NULL del esquema original.
        constructor.Property(especialidad => especialidad.Descripcion)
            .HasColumnName("Descripcion")
            .HasColumnType("nvarchar(max)");

        constructor.Property(especialidad => especialidad.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(especialidad => especialidad.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();
    }
}
