namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones;

/// <summary>
/// Contiene las clasificaciones general y específica encontradas para un territorio.
/// </summary>
/// <param name="General">Clasificación general (sub especialidad nula), si existe.</param>
/// <param name="Especifico">Clasificación específica de la sub especialidad solicitada, si existe.</param>
public sealed record ResolverTerritorioResultado(
    MaestraDespachoClasificacion? General,
    MaestraDespachoClasificacion? Especifico);
