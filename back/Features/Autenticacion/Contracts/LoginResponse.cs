namespace Runpd.Api.Features.Autenticacion.Contracts;

/// <summary>
/// Representa el token de acceso emitido al iniciar sesión correctamente.
/// </summary>
/// <param name="AccessToken">JWT firmado para autenticar solicitudes.</param>
/// <param name="TokenType">Esquema que se debe usar en el encabezado Authorization.</param>
/// <param name="ExpiraUtc">Fecha y hora UTC en que expira el token.</param>
public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiraUtc);
