namespace Runpd.Api.Features.Despachos;

/// <summary>
/// Indica el resultado de una operación sobre despachos.
/// </summary>
public enum EstadoOperacionDespacho
{
    /// <summary>La operación se completó correctamente.</summary>
    Ok,

    /// <summary>El despacho no existe.</summary>
    NoEncontrado,

    /// <summary>El código ya pertenece a otro despacho.</summary>
    CodigoDuplicado,

    /// <summary>Una o más relaciones no existen o están inactivas.</summary>
    RelacionInvalida
}

/// <summary>
/// Resultado de una operación sobre despachos.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Despacho">Despacho resultante cuando el estado es <see cref="EstadoOperacionDespacho.Ok"/>.</param>
public sealed record ResultadoDespacho(EstadoOperacionDespacho Estado, Despacho? Despacho);
