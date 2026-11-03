using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Municipios;

/// <summary>
/// Ejecuta las consultas del catálogo de municipios.
/// </summary>
public sealed class MunicipiosService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de municipios.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public MunicipiosService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene los municipios activos de un departamento. La consulta siempre se acota a un departamento
    /// porque el catálogo completo tiene más de mil registros.
    /// </summary>
    /// <param name="departamentoId">Identificador del departamento.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de municipios activos del departamento ordenada por nombre.</returns>
    public async Task<List<MaestraMunicipio>> ObtenerActivosPorDepartamentoAsync(
        Guid departamentoId,
        CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraMunicipios
            .AsNoTracking()
            .Where(municipio => municipio.Activo && municipio.DepartamentoId == departamentoId)
            .OrderBy(municipio => municipio.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
