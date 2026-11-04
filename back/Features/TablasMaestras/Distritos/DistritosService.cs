using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Distritos;

/// <summary>
/// Ejecuta las consultas del catálogo de distritos.
/// </summary>
public sealed class DistritosService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de distritos.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public DistritosService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene los distritos activos. Los inactivos se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de distritos activos ordenada por nombre.</returns>
    public async Task<List<MaestraDistrito>> ObtenerActivosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraDistritos
            .AsNoTracking()
            .Where(distrito => distrito.Activo)
            .OrderBy(distrito => distrito.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
