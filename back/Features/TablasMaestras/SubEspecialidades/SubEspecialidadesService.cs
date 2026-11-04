using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.SubEspecialidades;

/// <summary>
/// Ejecuta las consultas del catálogo de sub especialidades.
/// </summary>
public sealed class SubEspecialidadesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de sub especialidades.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public SubEspecialidadesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene las sub especialidades activas. Las inactivas se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de sub especialidades activas ordenada por nombre.</returns>
    public async Task<List<MaestraSubEspecialidad>> ObtenerActivasAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraSubEspecialidades
            .AsNoTracking()
            .Where(subEspecialidad => subEspecialidad.Activo)
            .OrderBy(subEspecialidad => subEspecialidad.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
