using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runpd.Api.Features.Roles;
using Runpd.Api.Features.Usuarios.Contracts;

namespace Runpd.Api.Features.Usuarios;

/// <summary>
/// Expone los endpoints HTTP para administrar usuarios.
/// </summary>
[ApiController]
[Authorize(Roles = RolesConocidos.AdministradorDelSistema)]
[Route("api/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly UsuariosService _servicio;

    /// <summary>
    /// Inicializa el controlador de usuarios.
    /// </summary>
    /// <param name="servicio">Servicio que ejecuta las operaciones de usuarios.</param>
    public UsuariosController(UsuariosService servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Lista todos los usuarios, incluidos los desactivados.
    /// </summary>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>La lista de usuarios.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UsuarioResponse>>> Listar(CancellationToken tokenCancelacion)
    {
        var usuarios = await _servicio.ObtenerTodosAsync(tokenCancelacion);
        return Ok(usuarios.Select(ConvertirRespuesta));
    }

    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario encontrado o una respuesta 404.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObtenerPorId(Guid id, CancellationToken tokenCancelacion)
    {
        var usuario = await _servicio.ObtenerPorIdAsync(id, tokenCancelacion);
        if (usuario is null)
        {
            return NotFound(new { mensaje = "No se encontró el usuario solicitado." });
        }

        return Ok(ConvertirRespuesta(usuario));
    }

    /// <summary>
    /// Crea un usuario nuevo, con o sin roles según la solicitud.
    /// </summary>
    /// <param name="solicitud">Datos del usuario; los roles son opcionales.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario creado, 400 si algún rol no es válido o 409 si el correo ya está registrado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Crear(
        CreateUsuarioRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.CrearAsync(solicitud, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoCrearUsuario.Ok => CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Usuario!.Id },
                ConvertirRespuesta(resultado.Usuario!)),
            EstadoCrearUsuario.CorreoDuplicado => Conflict(
                new { mensaje = "Ya existe un usuario con ese correo electrónico." }),
            EstadoCrearUsuario.RolInvalido => BadRequest(
                new { mensaje = "Uno o más roles no existen o están inactivos." }),
            _ => throw new InvalidOperationException(
                $"Estado de creación de usuario no controlado: {resultado.Estado}.")
        };
    }

    /// <summary>
    /// Actualiza los datos de un usuario y, opcionalmente, reemplaza sus roles.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="solicitud">Nuevos datos del usuario; los roles solo cambian si se envían.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario actualizado, 404 si no existe, 400 si algún rol no es válido
    /// o 409 si el correo está duplicado o dejaría el sistema sin administradores.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Actualizar(
        Guid id,
        UpdateUsuarioRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.ActualizarAsync(id, solicitud, tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoActualizarUsuario.Ok => Ok(ConvertirRespuesta(resultado.Usuario!)),
            EstadoActualizarUsuario.UsuarioNoEncontrado => NotFound(
                new { mensaje = "No se encontró el usuario solicitado." }),
            EstadoActualizarUsuario.CorreoDuplicado => Conflict(
                new { mensaje = "Ya existe otro usuario con ese correo electrónico." }),
            EstadoActualizarUsuario.RolInvalido => BadRequest(
                new { mensaje = "Uno o más roles no existen o están inactivos." }),
            EstadoActualizarUsuario.SinAdministrador => Conflict(
                new { mensaje = "La operación dejaría el sistema sin administradores activos." }),
            _ => throw new InvalidOperationException(
                $"Estado de actualización de usuario no controlado: {resultado.Estado}.")
        };
    }

    /// <summary>
    /// Cambia el estado activo de un usuario sin eliminar su registro.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="activo">Nuevo estado activo: <see langword="true"/> o <see langword="false"/>.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario actualizado, 404 si no existe o 409 si dejaría el sistema sin administradores.</returns>
    [HttpPatch("{id:guid}/activo/{activo:bool}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> CambiarActivo(
        Guid id,
        bool activo,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.CambiarActivoAsync(
            id,
            activo,
            tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoCambiarActivo.Ok => Ok(ConvertirRespuesta(resultado.Usuario!)),
            EstadoCambiarActivo.UsuarioNoEncontrado => NotFound(
                new { mensaje = "No se encontró el usuario solicitado." }),
            EstadoCambiarActivo.SinAdministrador => Conflict(
                new { mensaje = "No se puede desactivar el último administrador activo del sistema." }),
            _ => throw new InvalidOperationException(
                $"Estado de cambio de activo no controlado: {resultado.Estado}.")
        };
    }

    /// <summary>
    /// Reemplaza los roles activos asignados a un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="solicitud">Identificadores de los roles que debe tener el usuario.</param>
    /// <param name="tokenCancelacion">Token para cancelar la operación.</param>
    /// <returns>El usuario actualizado, 400 si algún rol no es válido, 404 si no existe el usuario
    /// o 409 si dejaría el sistema sin administradores.</returns>
    [HttpPut("{id:guid}/roles")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> ActualizarRoles(
        Guid id,
        UpdateRolesRequest solicitud,
        CancellationToken tokenCancelacion)
    {
        var resultado = await _servicio.ActualizarRolesAsync(
            id,
            solicitud.Roles,
            tokenCancelacion);

        return resultado.Estado switch
        {
            EstadoCambiarRoles.Ok => Ok(ConvertirRespuesta(resultado.Usuario!)),
            EstadoCambiarRoles.UsuarioNoEncontrado => NotFound(
                new { mensaje = "No se encontró el usuario solicitado." }),
            EstadoCambiarRoles.RolInvalido => BadRequest(
                new { mensaje = "Uno o más roles no existen o están inactivos." }),
            EstadoCambiarRoles.SinAdministrador => Conflict(
                new { mensaje = "La operación dejaría el sistema sin administradores activos." }),
            _ => throw new InvalidOperationException(
                $"Estado de cambio de roles no controlado: {resultado.Estado}.")
        };
    }

    private static UsuarioResponse ConvertirRespuesta(Usuario usuario)
    {
        var roles = usuario.Roles
            .OrderBy(asignacion => asignacion.Rol.Nombre)
            .Select(asignacion => new RolAsignadoResponse(
                asignacion.RolId,
                asignacion.Rol.Nombre))
            .ToList();

        return new UsuarioResponse(
            usuario.Id,
            usuario.PrimerNombre,
            usuario.SegundoNombre,
            usuario.PrimerApellido,
            usuario.SegundoApellido,
            usuario.CorreoElectronico,
            usuario.Activo,
            usuario.FechaCreacionUtc,
            roles);
    }
}
