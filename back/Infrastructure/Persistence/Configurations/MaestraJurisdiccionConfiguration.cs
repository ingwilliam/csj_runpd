using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de jurisdicciones al esquema de SQL Server.
/// </summary>
public sealed class MaestraJurisdiccionConfiguration : IEntityTypeConfiguration<MaestraJurisdiccion>
{
    /// <summary>
    /// Define los nombres y restricciones de la tabla de jurisdicciones.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraJurisdiccion> constructor)
    {
        constructor.ToTable("MaestraJurisdicciones");

        constructor.HasKey(jurisdiccion => jurisdiccion.Id);

        constructor.Property(jurisdiccion => jurisdiccion.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(jurisdiccion => jurisdiccion.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(jurisdiccion => jurisdiccion.Funcion)
            .HasColumnName("Funcion")
            .IsRequired();

        constructor.Property(jurisdiccion => jurisdiccion.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(jurisdiccion => jurisdiccion.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();
    }
}
