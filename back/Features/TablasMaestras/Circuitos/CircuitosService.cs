using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Circuitos;

/// <summary>
/// Ejecuta las consultas del catálogo de circuitos.
/// </summary>
public sealed class CircuitosService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de circuitos.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public CircuitosService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene los circuitos activos. Los inactivos se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de circuitos activos ordenada por nombre.</returns>
    public async Task<List<MaestraCircuito>> ObtenerActivosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraCircuitos
            .AsNoTracking()
            .Where(circuito => circuito.Activo)
            .OrderBy(circuito => circuito.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
