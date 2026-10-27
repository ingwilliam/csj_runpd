using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Features.Perfil;

/// <summary>
/// Indica el resultado de una operación sobre el perfil.
/// </summary>
public enum EstadoPerfil
{
    /// <summary>La operación se completó.</summary>
    Ok,

    /// <summary>El usuario no existe o está inactivo.</summary>
    UsuarioNoEncontrado,

    /// <summary>La contraseña actual no coincide con la registrada.</summary>
    ContrasenaActualIncorrecta
}

/// <summary>
/// Resultado de una operación sobre el perfil.
/// </summary>
/// <param name="Estado">Estado final de la operación.</param>
/// <param name="Usuario">Usuario con sus roles cuando el estado es <see cref="EstadoPerfil.Ok"/>.</param>
public sealed record PerfilResultado(EstadoPerfil Estado, Usuario? Usuario);
