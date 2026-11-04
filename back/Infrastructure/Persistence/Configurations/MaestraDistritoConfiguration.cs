using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Distritos;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de distritos al esquema de SQL Server y sus relaciones con jurisdicciones y sub especialidades.
/// </summary>
public sealed class MaestraDistritoConfiguration : IEntityTypeConfiguration<MaestraDistrito>
{
    /// <summary>
    /// Define los nombres, restricciones y las relaciones del distrito.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraDistrito> constructor)
    {
        constructor.ToTable("MaestraDistritos");

        constructor.HasKey(distrito => distrito.Id);

        constructor.Property(distrito => distrito.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(distrito => distrito.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(distrito => distrito.JurisdiccionId)
            .HasColumnName("JurisdiccionId")
            .IsRequired();

        constructor.Property(distrito => distrito.SubEspecialidadId)
            .HasColumnName("SubEspecialidadId");

        constructor.Property(distrito => distrito.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(distrito => distrito.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Restrict: no se puede borrar una jurisdicción que tenga distritos.
        constructor.HasOne<MaestraJurisdiccion>()
            .WithMany()
            .HasForeignKey(distrito => distrito.JurisdiccionId)
            .OnDelete(DeleteBehavior.Restrict);

        // SetNull: la sub especialidad es opcional; al borrarla, el distrito queda sin asociación en lugar de bloquear el borrado.
        constructor.HasOne<MaestraSubEspecialidad>()
            .WithMany()
            .HasForeignKey(distrito => distrito.SubEspecialidadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
