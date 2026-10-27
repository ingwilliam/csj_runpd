using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Features.Roles;

/// <summary>
/// Representa la asignación de un rol a un usuario.
/// </summary>
public sealed class UsuarioRol
{
    private UsuarioRol()
    {
    }

    /// <summary>
    /// Crea la asignación entre un usuario y un rol.
    /// </summary>
    /// <param name="usuario">Usuario al que se asigna el rol.</param>
    /// <param name="rol">Rol que se asigna.</param>
    public UsuarioRol(Usuario usuario, Rol rol)
    {
        Usuario = usuario;
        UsuarioId = usuario.Id;
        Rol = rol;
        RolId = rol.Id;
    }

    /// <summary>
    /// Obtiene el identificador del usuario.
    /// </summary>
    public Guid UsuarioId { get; private set; }

    /// <summary>
    /// Obtiene el identificador del rol.
    /// </summary>
    public Guid RolId { get; private set; }

    /// <summary>
    /// Obtiene el usuario de la asignación.
    /// </summary>
    public Usuario Usuario { get; private set; } = null!;

    /// <summary>
    /// Obtiene el rol de la asignación.
    /// </summary>
    public Rol Rol { get; private set; } = null!;
}
