using System.ComponentModel.DataAnnotations;

namespace Runpd.Api.Features.Usuarios.Contracts;

/// <summary>
/// Representa el conjunto de roles que debe tener un usuario.
/// </summary>
public sealed class UpdateRolesRequest
{
    /// <summary>
    /// Obtiene o establece los identificadores de los roles activos que se asignarán.
    /// </summary>
    [Required(ErrorMessage = "La lista de roles es obligatoria.")]
    public IReadOnlyCollection<Guid> Roles { get; init; } = Array.Empty<Guid>();
}
