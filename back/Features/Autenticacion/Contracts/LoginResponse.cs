namespace Runpd.Api.Features.Autenticacion.Contracts;

/// <summary>
/// Representa el token de acceso emitido al iniciar sesión correctamente.
/// </summary>
/// <param name="AccessToken">JWT firmado para autenticar solicitudes.</param>
/// <param name="TokenType">Esquema que se debe usar en el encabezado Authorization.</param>
/// <param name="ExpiraUtc">Fecha y hora UTC en que expira el token.</param>
/// <param name="DebeCambiarContrasena">Indica si el usuario entr\u00f3 con una contrase\u00f1a temporal y debe cambiarla.</param>
public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiraUtc,
    bool DebeCambiarContrasena);
