using Microsoft.EntityFrameworkCore;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.TablasMaestras.Departamentos;

/// <summary>
/// Ejecuta las consultas del catálogo de departamentos.
/// </summary>
public sealed class DepartamentosService
{
    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de departamentos.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public DepartamentosService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene los departamentos activos. Los inactivos se excluyen porque son de baja.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de departamentos activos ordenada por nombre.</returns>
    public async Task<List<MaestraDepartamento>> ObtenerActivosAsync(CancellationToken tokenCancelacion)
    {
        return await _contexto.MaestraDepartamentos
            .AsNoTracking()
            .Where(departamento => departamento.Activo)
            .OrderBy(departamento => departamento.Nombre)
            .ToListAsync(tokenCancelacion);
    }
}
