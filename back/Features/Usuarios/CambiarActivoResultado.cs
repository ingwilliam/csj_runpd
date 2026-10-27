namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Indica el resultado de intentar cambiar el estado activo de un usuario.
/// </summary>
public enum EstadoCambiarActivo
{
    /// <summary>
    /// El estado se actualizó correctamente.
    /// </summary>
    Ok,

    /// <summary>
    /// El usuario no existe.
    /// </summary>
    UsuarioNoEncontrado,

    /// <summary>
    /// La operación dejaría al sistema sin administradores activos.
    /// </summary>
    SinAdministrador
}

/// <summary>
/// Resultado de cambiar el estado activo de un usuario.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Usuario">Usuario actualizado cuando el estado es <see cref="EstadoCambiarActivo.Ok"/>.</param>
public sealed record CambiarActivoResultado(EstadoCambiarActivo Estado, Usuario? Usuario);
