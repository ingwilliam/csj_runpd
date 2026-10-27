namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Contiene la configuración usada para firmar los tokens JWT.
/// </summary>
/// <param name="Secret">Clave secreta para firmar los tokens.</param>
/// <param name="Issuer">Emisor de los tokens.</param>
/// <param name="Audience">Audiencia válida de los tokens.</param>
/// <param name="ExpirationMinutes">Duración del token en minutos.</param>
public sealed record JwtSettings(
    string Secret,
    string Issuer,
    string Audience,
    int ExpirationMinutes);
