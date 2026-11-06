namespace Runpd.Api.Features.TablasMaestras.DespachosClasificaciones.Contracts;

/// <summary>
/// Representa el resultado de resolver la clasificación territorial de un despacho.
/// </summary>
/// <param name="General">Clasificación sin sub especialidad, si existe.</param>
/// <param name="Especifico">Clasificación con la sub especialidad solicitada, si existe.</param>
public sealed record ResolverTerritorioResponse(
    DespachoClasificacionResponse? General,
    DespachoClasificacionResponse? Especifico);
