using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Entidades;

/// <summary>
/// Ejecuta las consultas del catálogo de entidades.
/// </summary>
public sealed class EntidadesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de entidades.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public EntidadesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene las entidades activas. Las inactivas se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de entidades activas ordenada por nombre.</returns>
    public async Task<List<MaestraEntidad>> ObtenerActivasAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraEntidades
            .AsNoTracking()
            .Where(entidad => entidad.Activo)
            .OrderBy(entidad => entidad.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
