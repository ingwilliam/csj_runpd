using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.ConsejosSeccionales;

/// <summary>
/// Ejecuta las consultas del catálogo de consejos seccionales.
/// </summary>
public sealed class ConsejosSeccionalesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de consejos seccionales.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public ConsejosSeccionalesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene los consejos seccionales activos. Los inactivos se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de consejos activos ordenada por nombre.</returns>
    public async Task<List<MaestraConsejoSeccional>> ObtenerActivosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraConsejosSeccionales
            .AsNoTracking()
            .Where(consejo => consejo.Activo)
            .OrderBy(consejo => consejo.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
