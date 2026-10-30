using System.Net;
using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Runpd.Api.Features.Autenticacion.Correo;

/// <summary>
/// Envía correos a través de un servidor SMTP usando MailKit.
/// </summary>
public sealed class ServicioCorreoSmtp : IServicioCorreo
{
    private const string AsuntoContrasenaTemporal = "Contraseña temporal de RUNPD";
    private const string NombrePlantilla = "Plantillas.contrasena-temporal.html";
    private const string NombreLogo = "Plantillas.RUNPD.png";
    private const string IdentificadorLogo = "logo-runpd";
    private const string MarcadorNombre = "{{NOMBRE}}";
    private const string MarcadorContrasenaTemporal = "{{CONTRASENA_TEMPORAL}}";

    private readonly SmtpSettings _configuracion;

    /// <summary>
    /// Inicializa el servicio con la configuración del servidor SMTP.
    /// </summary>
    /// <param name="configuracion">Servidor, credenciales y remitente del correo.</param>
    public ServicioCorreoSmtp(SmtpSettings configuracion)
    {
        _configuracion = configuracion;
    }

    /// <summary>
    /// Construye el correo con la plantilla y lo envía al usuario.
    /// </summary>
    /// <param name="correoDestino">Correo electrónico del usuario.</param>
    /// <param name="primerNombre">Primer nombre del usuario, usado en el saludo.</param>
    /// <param name="contrasenaTemporal">Contraseña temporal en texto plano.</param>
    /// <param name="tokenCancelacion">Token para cancelar el envío.</param>
    /// <exception cref="MailKit.Net.Smtp.SmtpCommandException">El servidor SMTP rechazó un comando.</exception>
    /// <exception cref="MailKit.Security.AuthenticationException">Las credenciales SMTP no son válidas.</exception>
    /// <exception cref="System.IO.IOException">No se pudo comunicar con el servidor SMTP.</exception>
    public async Task EnviarContrasenaTemporalAsync(
        string correoDestino,
        string primerNombre,
        string contrasenaTemporal,
        CancellationToken tokenCancelacion)
    {
        // Paso 1: construir el mensaje con la plantilla y el logo embebido.
        var mensaje = ConstruirMensaje(correoDestino, primerNombre, contrasenaTemporal);

        // Paso 2: conectar, autenticar si hay credenciales, enviar y cerrar la sesión.
        using var cliente = new SmtpClient();
        var opcionesSeguridad = _configuracion.EnableSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;
        await cliente.ConnectAsync(_configuracion.Host, _configuracion.Port, opcionesSeguridad, tokenCancelacion);

        if (!string.IsNullOrWhiteSpace(_configuracion.User))
        {
            await cliente.AuthenticateAsync(_configuracion.User, _configuracion.Password, tokenCancelacion);
        }

        await cliente.SendAsync(mensaje, tokenCancelacion);
        await cliente.DisconnectAsync(true, tokenCancelacion);
    }

    /// <summary>
    /// Crea el mensaje MIME con el HTML de la plantilla y el logo como recurso vinculado.
    /// </summary>
    private MimeMessage ConstruirMensaje(string correoDestino, string primerNombre, string contrasenaTemporal)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(MailboxAddress.Parse(_configuracion.From));
        mensaje.To.Add(MailboxAddress.Parse(correoDestino));
        mensaje.Subject = AsuntoContrasenaTemporal;

        var cuerpo = new BodyBuilder
        {
            HtmlBody = ObtenerHtml(primerNombre, contrasenaTemporal),
        };

        // El logo se adjunta con un Content-ID para que la plantilla lo muestre con cid:logo-runpd.
        var logo = cuerpo.LinkedResources.Add(
            "RUNPD.png",
            ObtenerRecurso(NombreLogo),
            ContentType.Parse("image/png"));
        logo.ContentId = IdentificadorLogo;

        mensaje.Body = cuerpo.ToMessageBody();
        return mensaje;
    }

    /// <summary>
    /// Rellena la plantilla HTML con el nombre y la contraseña, escapando ambos valores.
    /// </summary>
    private static string ObtenerHtml(string primerNombre, string contrasenaTemporal)
    {
        using var lector = new StreamReader(ObtenerRecurso(NombrePlantilla));
        var html = lector.ReadToEnd();

        // La contraseña se reemplaza primero: su alfabeto no contiene llaves, así que nunca
        // puede formar un marcador de otro dato.
        return html
            .Replace(MarcadorContrasenaTemporal, WebUtility.HtmlEncode(contrasenaTemporal))
            .Replace(MarcadorNombre, WebUtility.HtmlEncode(primerNombre));
    }

    /// <summary>
    /// Obtiene una copia en memoria de un recurso embebido en el ensamblado.
    /// </summary>
    /// <exception cref="InvalidOperationException">El recurso no existe en el ensamblado.</exception>
    private static MemoryStream ObtenerRecurso(string nombreRecurso)
    {
        using var origen = Assembly.GetExecutingAssembly().GetManifestResourceStream(nombreRecurso)
            ?? throw new InvalidOperationException(
                $"No se encontró la plantilla de correo '{nombreRecurso}' en el ensamblado.");

        var copia = new MemoryStream();
        origen.CopyTo(copia);
        copia.Position = 0;
        return copia;
    }
}
