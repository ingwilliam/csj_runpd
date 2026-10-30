namespace Runpd.Api.Features.Autenticacion.Correo;

/// <summary>
/// Envía los correos transaccionales del módulo de autenticación.
/// </summary>
public interface IServicioCorreo
{
    /// <summary>
    /// Envía al usuario el correo con su contraseña temporal.
    /// </summary>
    /// <param name="correoDestino">Correo electrónico del usuario.</param>
    /// <param name="primerNombre">Primer nombre del usuario, usado en el saludo.</param>
    /// <param name="contrasenaTemporal">Contraseña temporal en texto plano.</param>
    /// <param name="tokenCancelacion">Token para cancelar el envío.</param>
    /// <exception cref="MailKit.Net.Smtp.SmtpCommandException">El servidor SMTP rechazó un comando.</exception>
    /// <exception cref="MailKit.Security.AuthenticationException">Las credenciales SMTP no son válidas.</exception>
    /// <exception cref="System.IO.IOException">No se pudo comunicar con el servidor SMTP.</exception>
    Task EnviarContrasenaTemporalAsync(
        string correoDestino,
        string primerNombre,
        string contrasenaTemporal,
        CancellationToken tokenCancelacion);
}
