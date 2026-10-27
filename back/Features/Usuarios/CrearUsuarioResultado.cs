namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Indica el resultado de intentar crear un usuario.
/// </summary>
public enum EstadoCrearUsuario
{
    /// <summary>
    /// El usuario se creó correctamente.
    /// </summary>
    Ok,

    /// <summary>
    /// El correo electrónico ya está registrado.
    /// </summary>
    CorreoDuplicado,

    /// <summary>
    /// Uno o más roles solicitados no existen o están inactivos.
    /// </summary>
    RolInvalido
}

/// <summary>
/// Resultado de crear un usuario.
/// </summary>
/// <param name="Estado">Estado de la operación.</param>
/// <param name="Usuario">Usuario creado cuando el estado es <see cref="EstadoCrearUsuario.Ok"/>.</param>
public sealed record CrearUsuarioResultado(EstadoCrearUsuario Estado, Usuario? Usuario);
