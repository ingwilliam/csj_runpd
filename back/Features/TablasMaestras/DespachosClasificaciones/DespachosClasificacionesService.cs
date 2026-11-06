using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones;

/// <summary>
/// Ejecuta las consultas de clasificación territorial de despachos.
/// </summary>
public sealed class DespachosClasificacionesService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de clasificaciones de despachos.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public DespachosClasificacionesService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Resuelve la clasificación general y la específica de una sub especialidad para una jurisdicción y municipio.
    /// La general es la que no tiene sub especialidad; la específica solo se busca cuando se informa la sub especialidad.
    /// </summary>
    /// <param name="jurisdiccionId">Identificador de la jurisdicción.</param>
    /// <param name="municipioId">Identificador del municipio.</param>
    /// <param name="subEspecialidadId">Identificador de la sub especialidad, si aplica.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>Las clasificaciones general y específica encontradas, o nulas si no existen.</returns>
    public async Task<ResolverTerritorioResultado> ResolverTerritorioAsync(
        Guid jurisdiccionId,
        Guid municipioId,
        Guid? subEspecialidadId,
        CancellationToken tokenCancelacion)
    {
        var general = await _contexto.MaestraDespachosClasificaciones
            .AsNoTracking()
            .Where(clasificacion => clasificacion.JurisdiccionId == jurisdiccionId
                && clasificacion.MunicipioId == municipioId
                && clasificacion.SubEspecialidadId == null
                && clasificacion.Activo)
            .OrderBy(clasificacion => clasificacion.Id)
            .FirstOrDefaultAsync(tokenCancelacion);

        MaestraDespachoClasificacion? especifico = null;
        if (subEspecialidadId is not null)
        {
            especifico = await _contexto.MaestraDespachosClasificaciones
                .AsNoTracking()
                .Where(clasificacion => clasificacion.JurisdiccionId == jurisdiccionId
                    && clasificacion.MunicipioId == municipioId
                    && clasificacion.SubEspecialidadId == subEspecialidadId
                    && clasificacion.Activo)
                .OrderBy(clasificacion => clasificacion.Id)
                .FirstOrDefaultAsync(tokenCancelacion);
        }

        return new ResolverTerritorioResultado(general, especifico);
    }
}
