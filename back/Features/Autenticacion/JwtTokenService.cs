using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Runpd.Api.Features.Usuarios;

namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Genera tokens JWT firmados para usuarios autenticados.
/// </summary>
public sealed class JwtTokenService
{
    private readonly JwtSettings _configuracion;
    private readonly SigningCredentials _credencialesFirma;

    /// <summary>
    /// Inicializa el generador de tokens con la clave configurada.
    /// </summary>
    /// <param name="configuracion">Emisor, audiencia, vigencia y clave de firma.</param>
    public JwtTokenService(JwtSettings configuracion)
    {
        _configuracion = configuracion;
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracion.Secret));
        _credencialesFirma = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// Emite un token con la identidad y los roles activos del usuario.
    /// </summary>
    /// <param name="usuario">Usuario autenticado al que pertenece el token.</param>
    /// <returns>Token firmado y fecha de expiración UTC.</returns>
    public (string Token, DateTime ExpiraUtc) Generar(Usuario usuario)
    {
        var ahoraUtc = DateTime.UtcNow;
        var expiraUtc = ahoraUtc.AddMinutes(_configuracion.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new("sub", usuario.Id.ToString()),
            new("email", usuario.CorreoElectronico),
            new("jti", Guid.NewGuid().ToString())
        };

        claims.AddRange(usuario.Roles
            .Where(asignacion => asignacion.Rol.Activo)
            .Select(asignacion => new Claim("role", asignacion.Rol.Nombre)));

        var token = new JwtSecurityToken(
            issuer: _configuracion.Issuer,
            audience: _configuracion.Audience,
            claims: claims,
            notBefore: ahoraUtc,
            expires: expiraUtc,
            signingCredentials: _credencialesFirma);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraUtc);
    }
}
