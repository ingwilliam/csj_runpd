using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.TablasMaestras.Circuitos;
using Runpd.Api.Features.TablasMaestras.DespachosClasificaciones;
using Runpd.Api.Features.TablasMaestras.Distritos;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones;
using Runpd.Api.Features.TablasMaestras.Municipios;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de las clasificaciones de despacho al esquema de SQL Server
/// y sus relaciones con las tablas maestras.
/// </summary>
public sealed class MaestraDespachoClasificacionConfiguration
    : IEntityTypeConfiguration<MaestraDespachoClasificacion>
{
    /// <summary>
    /// Define los nombres, restricciones, índice único y relaciones de la clasificación.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<MaestraDespachoClasificacion> constructor)
    {
        constructor.ToTable("MaestraDespachosClasificaciones");

        constructor.HasKey(clasificacion => clasificacion.Id);

        constructor.Property(clasificacion => clasificacion.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(clasificacion => clasificacion.JurisdiccionId)
            .HasColumnName("JurisdiccionId")
            .IsRequired();

        constructor.Property(clasificacion => clasificacion.MunicipioId)
            .HasColumnName("MunicipioId")
            .IsRequired();

        constructor.Property(clasificacion => clasificacion.SubEspecialidadId)
            .HasColumnName("SubEspecialidadId");

        constructor.Property(clasificacion => clasificacion.DistritoId)
            .HasColumnName("DistritoId")
            .IsRequired();

        constructor.Property(clasificacion => clasificacion.CircuitoId)
            .HasColumnName("CircuitoId");

        constructor.Property(clasificacion => clasificacion.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(clasificacion => clasificacion.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Una jurisdicción y municipio solo pueden tener una clasificación por sub especialidad.
        // EF agrega el filtro SubEspecialidadId IS NOT NULL, igual que el esquema original de PostgreSQL,
        // para que la sub especialidad sea opcional; el resolver devuelve una sola clasificación general.
        constructor.HasIndex(clasificacion => new
            {
                clasificacion.JurisdiccionId,
                clasificacion.MunicipioId,
                clasificacion.SubEspecialidadId
            })
            .IsUnique();

        // Restrict: no se puede borrar una jurisdicción o municipio que tenga clasificaciones.
        constructor.HasOne<MaestraJurisdiccion>()
            .WithMany()
            .HasForeignKey(clasificacion => clasificacion.JurisdiccionId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraMunicipio>()
            .WithMany()
            .HasForeignKey(clasificacion => clasificacion.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: no se puede borrar un distrito que tenga clasificaciones.
        constructor.HasOne<MaestraDistrito>()
            .WithMany()
            .HasForeignKey(clasificacion => clasificacion.DistritoId)
            .OnDelete(DeleteBehavior.Restrict);

        // SetNull: la sub especialidad y el circuito son opcionales; al borrarlos,
        // la clasificación queda sin asociación en lugar de bloquear el borrado.
        constructor.HasOne<MaestraSubEspecialidad>()
            .WithMany()
            .HasForeignKey(clasificacion => clasificacion.SubEspecialidadId)
            .OnDelete(DeleteBehavior.SetNull);

        constructor.HasOne<MaestraCircuito>()
            .WithMany()
            .HasForeignKey(clasificacion => clasificacion.CircuitoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
