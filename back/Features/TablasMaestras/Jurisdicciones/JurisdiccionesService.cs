using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Jurisdicciones;

/// <summary>
/// Ejecuta las consultas del catálogo de jurisdicciones.
/// </summary>
public sealed class JurisdiccionesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de jurisdicciones.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public JurisdiccionesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene las jurisdicciones activas. Las inactivas se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de jurisdicciones activas ordenada por nombre.</returns>
    public async Task<List<MaestraJurisdiccion>> ObtenerActivasAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraJurisdicciones
            .AsNoTracking()
            .Where(jurisdiccion => jurisdiccion.Activo)
            .OrderBy(jurisdiccion => jurisdiccion.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
