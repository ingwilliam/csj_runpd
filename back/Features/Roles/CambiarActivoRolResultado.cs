namespace Runpd.Api.Features.Roles;

/// <summary>
/// Indica el resultado de intentar cambiar el estado activo de un rol.
/// </summary>
public enum EstadoCambiarActivoRol
{
    /// <summary>
    /// El estado se actualizó correctamente.
    /// </summary>
    Ok,

    /// <summary>
    /// El rol no existe.
    /// </summary>
    NoEncontrado,

    /// <summary>
    /// El rol es del sistema o está asignado a usuarios y no puede desactivarse.
    /// </summary>
    EnUso
}

/// <summary>
/// Resultado de cambiar el estado activo de un rol.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Rol">Rol actualizado cuando el estado es <see cref="EstadoCambiarActivoRol.Ok"/>.</param>
public sealed record CambiarActivoRolResultado(EstadoCambiarActivoRol Estado, Rol? Rol);
