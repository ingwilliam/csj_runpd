namespace Runpd.Api.Features.Roles;

/// <summary>
/// Define los nombres de los roles del sistema y su catálogo inicial.
/// </summary>
public static class RolesConocidos
{
    /// <summary>
    /// Nombre del rol que administra todo el sistema.
    /// </summary>
    public const string AdministradorDelSistema = "Administrador del sistema";

    /// <summary>
    /// Nombre del rol que administra la UDAE.
    /// </summary>
    public const string AdministradorUDAE = "Administrador de la UDAE";

    /// <summary>
    /// Nombre del rol que administra el Consejo Seccional.
    /// </summary>
    public const string AdministradorConsejoSeccional = "Administrador del Consejo Seccional";

    /// <summary>
    /// Nombre del rol que administra el Despacho.
    /// </summary>
    public const string AdministradorDespacho = "Administrador del Despacho";

    /// <summary>
    /// Nombre del rol de consulta de información.
    /// </summary>
    public const string Consulta = "Consulta";

    /// <summary>
    /// Nombre del rol de generación de reportes.
    /// </summary>
    public const string Reportes = "Reportes";

    /// <summary>
    /// Obtiene los roles del sistema con la descripción usada al crearlos.
    /// </summary>
    public static IReadOnlyList<(string Nombre, string Descripcion)> Catalogo { get; } =
        new (string Nombre, string Descripcion)[]
        {
            (AdministradorDelSistema, "Administra usuarios, roles y la configuración general."),
            (AdministradorUDAE, "Administra la información de la UDAE."),
            (AdministradorConsejoSeccional, "Administra la información del Consejo Seccional."),
            (AdministradorDespacho, "Administra la información del Despacho."),
            (Consulta, "Consulta la información del sistema."),
            (Reportes, "Genera reportes del sistema.")
        };

    /// <summary>
    /// Obtiene los nombres de los roles del sistema que no pueden desactivarse.
    /// </summary>
    public static IReadOnlySet<string> RolesDelSistema { get; } =
        new HashSet<string>(
            Catalogo.Select(rol => rol.Nombre),
            StringComparer.OrdinalIgnoreCase);
}
