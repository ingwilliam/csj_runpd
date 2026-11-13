using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Runpd.Api.Features.Despachos;
using Runpd.Api.Features.TablasMaestras.Circuitos;
using Runpd.Api.Features.TablasMaestras.Distritos;
using Runpd.Api.Features.TablasMaestras.Entidades;
using Runpd.Api.Features.TablasMaestras.Especialidades;
using Runpd.Api.Features.TablasMaestras.Jurisdicciones;
using Runpd.Api.Features.TablasMaestras.Municipios;
using Runpd.Api.Features.TablasMaestras.SubEspecialidades;

namespace Runpd.Api.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configura el mapeo de despachos al esquema de SQL Server y sus relaciones con las tablas maestras.
/// </summary>
public sealed class DespachoConfiguration : IEntityTypeConfiguration<Despacho>
{
    /// <summary>
    /// Define los nombres, restricciones, índice único y relaciones del despacho.
    /// </summary>
    /// <param name="constructor">Constructor de configuración de la entidad.</param>
    public void Configure(EntityTypeBuilder<Despacho> constructor)
    {
        constructor.ToTable("Despachos");

        constructor.HasKey(despacho => despacho.Id);

        constructor.Property(despacho => despacho.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        constructor.Property(despacho => despacho.Codigo)
            .HasColumnName("Codigo")
            .HasMaxLength(50)
            .IsRequired();

        // El código identifica al despacho, por eso no puede repetirse.
        constructor.HasIndex(despacho => despacho.Codigo)
            .IsUnique();

        constructor.Property(despacho => despacho.Nombre)
            .HasColumnName("Nombre")
            .HasMaxLength(200)
            .IsRequired();

        constructor.Property(despacho => despacho.Estado)
            .HasColumnName("Estado")
            .HasMaxLength(30)
            .IsRequired();

        constructor.Property(despacho => despacho.Promiscuo)
            .HasColumnName("Promiscuo")
            .IsRequired();

        constructor.Property(despacho => despacho.EsDescongestion)
            .HasColumnName("EsDescongestion")
            .IsRequired()
            .HasDefaultValue(false);

        constructor.Property(despacho => despacho.Direccion)
            .HasColumnName("Direccion")
            .HasMaxLength(255);

        constructor.Property(despacho => despacho.ComplementoDireccion)
            .HasColumnName("ComplementoDireccion")
            .HasMaxLength(150);

        constructor.Property(despacho => despacho.TelefonoFijo)
            .HasColumnName("TelefonoFijo")
            .HasMaxLength(50);

        constructor.Property(despacho => despacho.TelefonoCelular)
            .HasColumnName("TelefonoCelular")
            .HasMaxLength(50);

        constructor.Property(despacho => despacho.Correo)
            .HasColumnName("Correo")
            .HasMaxLength(150);

        constructor.Property(despacho => despacho.HorarioAtencion)
            .HasColumnName("HorarioAtencion")
            .HasMaxLength(150);

        constructor.Property(despacho => despacho.EspecialidadId).HasColumnName("EspecialidadId");
        constructor.Property(despacho => despacho.SubEspecialidadId).HasColumnName("SubEspecialidadId");
        constructor.Property(despacho => despacho.MunicipioId).HasColumnName("MunicipioId").IsRequired();
        constructor.Property(despacho => despacho.JurisdiccionId).HasColumnName("JurisdiccionId").IsRequired();
        constructor.Property(despacho => despacho.DistritoId).HasColumnName("DistritoId");
        constructor.Property(despacho => despacho.CircuitoId).HasColumnName("CircuitoId");
        constructor.Property(despacho => despacho.DistritoSubEspecialidadId).HasColumnName("DistritoSubEspecialidadId");
        constructor.Property(despacho => despacho.CircuitoSubEspecialidadId).HasColumnName("CircuitoSubEspecialidadId");
        constructor.Property(despacho => despacho.EntidadId).HasColumnName("EntidadId");

        constructor.Property(despacho => despacho.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        constructor.Property(despacho => despacho.FechaCreacionUtc)
            .HasColumnName("FechaCreacionUtc")
            .HasColumnType("datetime2")
            .IsRequired();

        // Relaciones obligatorias y opcionales. Restrict impide borrar maestras en uso;
        // SetNull solo se usa en sub especialidad, como indica el esquema original.
        constructor.HasOne<MaestraEspecialidad>()
            .WithMany()
            .HasForeignKey(despacho => despacho.EspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraSubEspecialidad>()
            .WithMany()
            .HasForeignKey(despacho => despacho.SubEspecialidadId)
            .OnDelete(DeleteBehavior.SetNull);

        constructor.HasOne<MaestraMunicipio>()
            .WithMany()
            .HasForeignKey(despacho => despacho.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraJurisdiccion>()
            .WithMany()
            .HasForeignKey(despacho => despacho.JurisdiccionId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraDistrito>()
            .WithMany()
            .HasForeignKey(despacho => despacho.DistritoId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraCircuito>()
            .WithMany()
            .HasForeignKey(despacho => despacho.CircuitoId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraDistrito>()
            .WithMany()
            .HasForeignKey(despacho => despacho.DistritoSubEspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraCircuito>()
            .WithMany()
            .HasForeignKey(despacho => despacho.CircuitoSubEspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);

        constructor.HasOne<MaestraEntidad>()
            .WithMany()
            .HasForeignKey(despacho => despacho.EntidadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
