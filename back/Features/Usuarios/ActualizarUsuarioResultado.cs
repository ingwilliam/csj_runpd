namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Indica el resultado de intentar actualizar un usuario.
/// </summary>
public enum EstadoActualizarUsuario
{
    /// <summary>
    /// El usuario se actualizó correctamente.
    /// </summary>
    Ok,

    /// <summary>
    /// El usuario no existe.
    /// </summary>
    UsuarioNoEncontrado,

    /// <summary>
    /// El correo electrónico ya pertenece a otro usuario.
    /// </summary>
    CorreoDuplicado,

    /// <summary>
    /// Uno o más roles solicitados no existen o están inactivos.
    /// </summary>
    RolInvalido,

    /// <summary>
    /// La operación dejaría al sistema sin administradores activos.
    /// </summary>
    SinAdministrador
}

/// <summary>
/// Resultado de actualizar un usuario.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Usuario">Usuario actualizado cuando el estado es <see cref="EstadoActualizarUsuario.Ok"/>.</param>
public sealed record ActualizarUsuarioResultado(EstadoActualizarUsuario Estado, Usuario? Usuario);
