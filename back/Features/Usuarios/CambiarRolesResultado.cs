namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Indica el resultado de intentar reemplazar los roles de un usuario.
/// </summary>
public enum EstadoCambiarRoles
{
    /// <summary>
    /// Los roles se actualizaron correctamente.
    /// </summary>
    Ok,

    /// <summary>
    /// El usuario no existe.
    /// </summary>
    UsuarioNoEncontrado,

    /// <summary>
    /// Uno o más roles no existen o están inactivos.
    /// </summary>
    RolInvalido,

    /// <summary>
    /// La operación dejaría al sistema sin administradores activos.
    /// </summary>
    SinAdministrador
}

/// <summary>
/// Resultado de reemplazar los roles de un usuario.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Usuario">Usuario actualizado cuando el estado es <see cref="EstadoCambiarRoles.Ok"/>.</param>
public sealed record CambiarRolesResultado(EstadoCambiarRoles Estado, Usuario? Usuario);
