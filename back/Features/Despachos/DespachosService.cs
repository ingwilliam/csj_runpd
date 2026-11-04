using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Runpd.Api.Features.Despachos.Contracts;
using Runpd.Api.Infrastructure.Persistence;

namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Ejecuta las operaciones de administración de despachos.
/// </summary>
public sealed class DespachosService
{
    private const string CollationBusqueda = "Latin1_General_CI_AI";

    private readonly RunpdDbContext _contexto;

    /// <summary>
    /// Inicializa el servicio de despachos.
    /// </summary>
    /// <param name="contexto">Contexto de acceso a la base de datos.</param>
    public DespachosService(RunpdDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Obtiene una página de despachos, aplicando búsqueda por código o nombre y filtro de activos.
    /// </summary>
    /// <param name="consulta">Filtros y paginación solicitados.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La página de despachos y los totales del listado.</returns>
    public async Task<DespachosPaginadosResultado> ObtenerPaginadosAsync(
        ListarDespachosQuery consulta,
        CancellationToken tokenCancelacion)
    {
        var consultaDespachos = _contexto.Despachos.AsNoTracking();

        // Paso 1: filtrar por activos o inactivos.
        if (consulta.Estado == EstadoDespachoFiltro.Activos)
        {
            consultaDespachos = consultaDespachos.Where(despacho => despacho.Activo);
        }
        else if (consulta.Estado == EstadoDespachoFiltro.Inactivos)
        {
            consultaDespachos = consultaDespachos.Where(despacho => !despacho.Activo);
        }

        // Paso 2: cada palabra debe coincidir con el código o el nombre.
        // La colación mantiene la búsqueda sin distinguir mayúsculas ni tildes.
        var terminos = (consulta.Busqueda ?? string.Empty)
            .Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var termino in terminos)
        {
            consultaDespachos = consultaDespachos.Where(despacho =>
                EF.Functions.Collate(despacho.Codigo, CollationBusqueda).Contains(termino)
                || EF.Functions.Collate(despacho.Nombre, CollationBusqueda).Contains(termino));
        }

        // Paso 3: contar el resultado filtrado y los totales globales.
        var total = await consultaDespachos.CountAsync(tokenCancelacion);
        var totalDespachos = await _contexto.Despachos.CountAsync(tokenCancelacion);
        var totalActivos = await _contexto.Despachos.CountAsync(despacho => despacho.Activo, tokenCancelacion);
        var totalPaginas = total == 0
            ? 0
            : (int)Math.Ceiling(total / (double)consulta.Tamano);

        // Paso 4: obtener la página solicitada, ordenada por código para una paginación estable.
        var despachos = await consultaDespachos
            .OrderBy(despacho => despacho.Codigo)
            .ThenBy(despacho => despacho.Id)
            .Skip((consulta.Pagina - 1) * consulta.Tamano)
            .Take(consulta.Tamano)
            .ToListAsync(tokenCancelacion);

        return new DespachosPaginadosResultado(
            despachos,
            consulta.Pagina,
            consulta.Tamano,
            total,
            totalPaginas,
            totalDespachos,
            totalActivos,
            totalDespachos - totalActivos);
    }

    /// <summary>
    /// Busca un despacho por su identificador.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El despacho encontrado o <see langword="null"/>.</returns>
    public async Task<Despacho?> ObtenerPorIdAsync(Guid id, CancellationToken tokenCancelacion)
    {
        return await _contexto.Despachos
            .AsNoTracking()
            .SingleOrDefaultAsync(despacho => despacho.Id == id, tokenCancelacion);
    }

    /// <summary>
    /// Crea un despacho después de validar sus relaciones.
    /// </summary>
    /// <param name="solicitud">Datos del despacho a crear.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el despacho creado o el error de validación.</returns>
    public async Task<ResultadoDespacho> CrearAsync(
        DespachoRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var datos = ConvertirDatos(solicitud);

        // Valida las relaciones antes de insertar para no dejar datos parciales.
        if (!await RelacionesValidasAsync(datos, tokenCancelacion))
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.RelacionInvalida, null);
        }

        var despacho = new Despacho(datos);
        _contexto.Despachos.Add(despacho);

        try
        {
            await _contexto.SaveChangesAsync(tokenCancelacion);
            return new ResultadoDespacho(EstadoOperacionDespacho.Ok, despacho);
        }
        catch (DbUpdateException excepcion) when (EsViolacionDeIndiceUnico(excepcion))
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.CodigoDuplicado, null);
        }
    }

    /// <summary>
    /// Actualiza los datos de un despacho después de validar sus relaciones.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="solicitud">Nuevos datos del despacho.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el despacho actualizado o el error de validación.</returns>
    public async Task<ResultadoDespacho> ActualizarAsync(
        Guid id,
        DespachoRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var despacho = await _contexto.Despachos
            .SingleOrDefaultAsync(despachoActual => despachoActual.Id == id, tokenCancelacion);

        if (despacho is null)
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.NoEncontrado, null);
        }

        var datos = ConvertirDatos(solicitud);
        if (!await RelacionesValidasAsync(datos, tokenCancelacion))
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.RelacionInvalida, null);
        }

        despacho.Actualizar(datos);

        try
        {
            await _contexto.SaveChangesAsync(tokenCancelacion);
            return new ResultadoDespacho(EstadoOperacionDespacho.Ok, despacho);
        }
        catch (DbUpdateException excepcion) when (EsViolacionDeIndiceUnico(excepcion))
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.CodigoDuplicado, null);
        }
    }

    /// <summary>
    /// Cambia el estado activo de un despacho sin borrar sus datos.
    /// </summary>
    /// <param name="id">Identificador del despacho.</param>
    /// <param name="activo">Nuevo estado activo.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El resultado con el despacho actualizado o <c>NoEncontrado</c>.</returns>
    public async Task<ResultadoDespacho> CambiarActivoAsync(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var despacho = await _contexto.Despachos
            .SingleOrDefaultAsync(despachoActual => despachoActual.Id == id, tokenCancelacion);

        if (despacho is null)
        {
            return new ResultadoDespacho(EstadoOperacionDespacho.NoEncontrado, null);
        }

        despacho.EstablecerActivo(activo);
        await _contexto.SaveChangesAsync(tokenCancelacion);

        return new ResultadoDespacho(EstadoOperacionDespacho.Ok, despacho);
    }

    // Convierte la solicitud en datos normalizados: recorta textos y deja nulos los opcionales vacíos.
    private static DatosDespacho ConvertirDatos(DespachoRequest solicitud)
    {
        return new DatosDespacho(
            solicitud.Codigo.Trim(),
            solicitud.Nombre.Trim(),
            solicitud.Estado,
            solicitud.Promiscuo,
            NormalizarTextoOpcional(solicitud.Direccion),
            NormalizarTextoOpcional(solicitud.ComplementoDireccion),
            NormalizarTextoOpcional(solicitud.TelefonoFijo),
            NormalizarTextoOpcional(solicitud.TelefonoCelular),
            NormalizarTextoOpcional(solicitud.Correo),
            NormalizarTextoOpcional(solicitud.HorarioAtencion),
            solicitud.EspecialidadId,
            solicitud.SubEspecialidadId,
            solicitud.MunicipioId,
            solicitud.JurisdiccionId,
            solicitud.DistritoId,
            solicitud.CircuitoId,
            solicitud.DistritoSubEspecialidadId,
            solicitud.CircuitoSubEspecialidadId,
            solicitud.EntidadId);
    }

    // Comprueba que cada relación informada exista y esté activa. Las relaciones opcionales vacías se aceptan.
    private async Task<bool> RelacionesValidasAsync(DatosDespacho datos, CancellationToken tokenCancelacion)
    {
        return await EsRelacionValidaAsync(_contexto.MaestraEspecialidades, datos.EspecialidadId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraSubEspecialidades, datos.SubEspecialidadId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraMunicipios, datos.MunicipioId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraJurisdicciones, datos.JurisdiccionId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraDistritos, datos.DistritoId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraCircuitos, datos.CircuitoId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraDistritos, datos.DistritoSubEspecialidadId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraCircuitos, datos.CircuitoSubEspecialidadId, tokenCancelacion)
            && await EsRelacionValidaAsync(_contexto.MaestraEntidades, datos.EntidadId, tokenCancelacion);
    }

    // Una relación es válida si no se informó (opcional) o si existe un registro activo con ese identificador.
    private static async Task<bool> EsRelacionValidaAsync<T>(
        DbSet<T> conjunto,
        Guid? id,
        CancellationToken tokenCancelacion) where T : class
    {
        if (id is null)
        {
            return true;
        }

        return await conjunto.AnyAsync(
            registro => EF.Property<Guid>(registro, "Id") == id.Value
                && EF.Property<bool>(registro, "Activo"),
            tokenCancelacion);
    }

    private static string? NormalizarTextoOpcional(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

    private static bool EsViolacionDeIndiceUnico(DbUpdateException excepcion)
    {
        return excepcion.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
