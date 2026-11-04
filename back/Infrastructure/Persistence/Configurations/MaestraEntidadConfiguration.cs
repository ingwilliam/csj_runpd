using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Entidades;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de entidades al esquema de SQL Server y su relación con las jurisdicciones.
/// </summary>
public sealed class MaestraEntidadConfiguration : IEntityTypeConfiguration<MaestraEntidad>
{
    /// <summary>
    /// Define los nombres, restricciones y la relación con jurisdicciones.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraEntidad> constructor)
    {
        constructor.ToTable("MaestraEntidades");

        constructor.HasKey(entidad => entidad.Id);

        constructor.Property(entidad => entidad.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(entidad => entidad.Codigo)
            .HasColumnName("Codigo")
            .HasMaxLength(20)
            .IsUnicode(false)
            .IsRequired();

        constructor.Property(entidad => entidad.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(entidad => entidad.JurisdiccionId)
            .HasColumnName("JurisdiccionId")
            .IsRequired();

        constructor.Property(entidad => entidad.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(entidad => entidad.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Restrict: no se puede borrar una jurisdicción que tenga entidades.
        constructor.HasOne<MaestraJurisdiccion>()
            .WithMany()
            .HasForeignKey(entidad => entidad.JurisdiccionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
