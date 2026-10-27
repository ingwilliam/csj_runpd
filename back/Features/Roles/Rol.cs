namespace Runpd.Api.Features.Roles;

/// <summary>
/// Representa un rol asignable a los usuarios.
/// </summary>
public sealed class Rol
{
    private Rol()
    {
    }

    /// <summary>
    /// Crea un rol activo con sus datos iniciales.
    /// </summary>
    /// <param name="nombre">Nombre único del rol.</param>
    /// <param name="descripcion">Descripción opcional del rol.</param>
    public Rol(string nombre, string? descripcion)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = true;
        FechaCreacionUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene el identificador del rol.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Obtiene el nombre del rol.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Obtiene la descripción del rol.
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Obtiene si el rol está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Obtiene la fecha de creación del rol en UTC.
    /// </summary>
    public DateTime FechaCreacionUtc { get; private set; }

    /// <summary>
    /// Obtiene las asignaciones de este rol a usuarios.
    /// </summary>
    public ICollection<UsuarioRol> Usuarios { get; private set; } = new List<UsuarioRol>();

    /// <summary>
    /// Actualiza los datos editables del rol.
    /// </summary>
    /// <param name="nombre">Nuevo nombre único.</param>
    /// <param name="descripcion">Nueva descripción.</param>
    public void Actualizar(string nombre, string? descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    /// <summary>
    /// Establece el estado activo del rol.
    /// </summary>
    /// <param name="activo">Nuevo estado activo.</param>
    public void EstablecerActivo(bool activo)
    {
        Activo = activo;
    }
}
