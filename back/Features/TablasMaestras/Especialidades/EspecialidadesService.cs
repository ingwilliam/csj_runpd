using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Especialidades;

/// <summary>
/// Ejecuta las consultas del catálogo de especialidades.
/// </summary>
public sealed class EspecialidadesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de especialidades.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public EspecialidadesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene las especialidades activas. Las inactivas se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de especialidades activas ordenada por nombre.</returns>
    public async Task<List<MaestraEspecialidad>> ObtenerActivasAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraEspecialidades
            .AsNoTracking()
            .Where(especialidad => especialidad.Activo)
            .OrderBy(especialidad => especialidad.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
