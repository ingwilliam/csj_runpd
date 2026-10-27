using Runpd.Api.Features.Roles;

namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Representa un usuario almacenado en el sistema.
/// </summary>
public sealed class Usuario
{
    private Usuario()
    {
    }

    /// <summary>
    /// Crea un usuario con sus datos iniciales.
    /// </summary>
    /// <param name="primerNombre">Primer nombre del usuario.</param>
    /// <param name="segundoNombre">Segundo nombre del usuario, si lo tiene.</param>
    /// <param name="primerApellido">Primer apellido del usuario.</param>
    /// <param name="segundoApellido">Segundo apellido del usuario, si lo tiene.</param>
    /// <param name="correoElectronico">Correo electrónico normalizado.</param>
    public Usuario(
        string primerNombre,
        string? segundoNombre,
        string primerApellido,
        string? segundoApellido,
        string correoElectronico)
    {
        Id = Guid.NewGuid();
        PrimerNombre = primerNombre;
        SegundoNombre = segundoNombre;
        PrimerApellido = primerApellido;
        SegundoApellido = segundoApellido;
        CorreoElectronico = correoElectronico;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del usuario.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el primer nombre del usuario.
    /// </summary>
    public string PrimerNombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el segundo nombre del usuario, si lo tiene.
    /// </summary>
    public string? SegundoNombre { get; private set; }

    /// <summary>
    /// Obtiene el primer apellido del usuario.
    /// </summary>
    public string PrimerApellido { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el segundo apellido del usuario, si lo tiene.
    /// </summary>
    public string? SegundoApellido { get; private set; }

    /// <summary>
    /// Obtiene el correo electrónico del usuario. Es el identificador de inicio de sesión.
    /// </summary>
    public string CorreoElectronico { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene el hash de la contraseña del usuario. Nunca se almacena ni se expone la contraseña en texto plano.
    /// </summary>
    public string? Contrasena { get; private set; }

    /// <summary>
    /// Obtiene las asignaciones de roles del usuario.
    /// </summary>
    public ICollection<UsuarioRol> Roles { get; private set; } = new List<UsuarioRol>();

    /// <summary>
    /// Obtiene si el usuario está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del usuario en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }

    /// <summary>
    /// Actualiza los datos editables del usuario. El estado activo solo se cambia con
    /// <see cref="EstablecerActivo"/> y la contraseña solo con <see cref="EstablecerContrasena"/>.
    /// </summary>
    /// <param name="primerNombre">Nuevo primer nombre.</param>
    /// <param name="segundoNombre">Nuevo segundo nombre, si lo tiene.</param>
    /// <param name="primerApellido">Nuevo primer apellido.</param>
    /// <param name="segundoApellido">Nuevo segundo apellido, si lo tiene.</param>
    /// <param name="correoElectronico">Nuevo correo electrónico normalizado.</param>
    public void Actualizar(
        string primerNombre,
        string? segundoNombre,
        string primerApellido,
        string? segundoApellido,
        string correoElectronico)
    {
        PrimerNombre = primerNombre;
        SegundoNombre = segundoNombre;
        PrimerApellido = primerApellido;
        SegundoApellido = segundoApellido;
        CorreoElectronico = correoElectronico;
    }

    /// <summary>
    /// Establece el hash de la contraseña del usuario.
    /// </summary>
    /// <param name="contrasena">Hash calculado de la contraseña.</param>
    public void EstablecerContrasena(string contrasena)
    {
        Contrasena = contrasena;
    }

    /// <summary>
    /// Establece el estado activo del usuario.
    /// </summary>
    /// <param name="activo">Nuevo estado activo.</param>
    public void EstablecerActivo(bool activo)
    {
        Activo = activo;
    }
}
