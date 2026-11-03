using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.DireccionesSeccionales;

/// <summary>
/// Ejecuta las consultas del catálogo de direcciones seccionales.
/// </summary>
public sealed class DireccionesSeccionalesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de direcciones seccionales.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public DireccionesSeccionalesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene las direcciones seccionales activas. Las inactivas se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de direcciones activas ordenada por nombre.</returns>
    public async Task<List<MaestraDireccionSeccional>> ObtenerActivosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraDireccionesSeccionales
            .AsNoTracking()
            .Where(direccion => direccion.Activo)
            .OrderBy(direccion => direccion.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
