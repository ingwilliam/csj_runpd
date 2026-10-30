namespace Runpd.Api.Features.Autenticacion.Correo;

/// <summary>
/// Contiene la configuración del servidor SMTP usado para enviar correos.
/// </summary>
/// <param name="Host">Servidor SMTP.</param>
/// <param name="Port">Puerto del servidor SMTP.</param>
/// <param name="User">Usuario del servidor SMTP; <see langword="null"/> si no requiere autenticación.</param>
/// <param name="Password">Contraseña del servidor SMTP; cadena vacía si no requiere autenticación.</param>
/// <param name="From">Remitente que aparece en los correos enviados.</param>
/// <param name="EnableSsl">Indica si la conexión usa STARTTLS.</param>
public sealed record SmtpSettings(
    string Host,
    int Port,
    string? User,
    string Password,
    string From,
    bool EnableSsl);
