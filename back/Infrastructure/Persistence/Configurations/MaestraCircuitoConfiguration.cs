using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Circuitos;
using Runpd.Api.Features.TablasMaestras.Distritos;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de circuitos al esquema de SQL Server y sus relaciones con distritos y sub especialidades.
/// </summary>
public sealed class MaestraCircuitoConfiguration : IEntityTypeConfiguration<MaestraCircuito>
{
    /// <summary>
    /// Define los nombres, restricciones y las relaciones del circuito.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraCircuito> constructor)
    {
        constructor.ToTable("MaestraCircuitos");

        constructor.HasKey(circuito => circuito.Id);

        constructor.Property(circuito => circuito.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(circuito => circuito.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        constructor.Property(circuito => circuito.DistritoId)
            .HasColumnName("DistritoId")
            .IsRequired();

        constructor.Property(circuito => circuito.SubEspecialidadId)
            .HasColumnName("SubEspecialidadId");

        constructor.Property(circuito => circuito.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(circuito => circuito.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Restrict: no se puede borrar un distrito que tenga circuitos.
        constructor.HasOne<MaestraDistrito>()
            .WithMany()
            .HasForeignKey(circuito => circuito.DistritoId)
            .OnDelete(DeleteBehavior.Restrict);

        // SetNull: la sub especialidad es opcional; al borrarla, el circuito queda sin asociación en lugar de bloquear el borrado.
        constructor.HasOne<MaestraSubEspecialidad>()
            .WithMany()
            .HasForeignKey(circuito => circuito.SubEspecialidadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
