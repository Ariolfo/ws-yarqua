using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Infrastructure.Options;

namespace Yarqua.Infrastructure.Services;

/// <summary>
/// Emisión y validación de JWT HS256.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _key;

    /// <summary>
    /// Inicializa el servicio JWT.
    /// </summary>
    /// <param name="options">Opciones Jwt.</param>
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
    }

    /// <inheritdoc />
    public string CreateAccessToken(string userId, string? name = null) =>
        CreateToken(userId, "access", TimeSpan.FromMinutes(_options.AccessMinutes), name);

    /// <inheritdoc />
    public string CreateRefreshToken(string userId, string? name = null) =>
        CreateToken(userId, "refresh", TimeSpan.FromDays(_options.RefreshDays), name);

    /// <inheritdoc />
    public (string Sub, string? Name) ValidateToken(string token, string expectedType)
    {
        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        }, out var validated);

        var jwt = (JwtSecurityToken)validated;
        var type = jwt.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
        if (!string.Equals(type, expectedType, StringComparison.Ordinal))
        {
            throw new SecurityTokenException("Tipo de token inválido");
        }

        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? throw new SecurityTokenException("Token sin sub");
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        return (sub, name);
    }

    private string CreateToken(string userId, string type, TimeSpan lifetime, string? name)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new("type", type),
            new(JwtRegisteredClaimNames.Iat, Epoch(now).ToString(), ClaimValueTypes.Integer64),
        };

        if (!string.IsNullOrWhiteSpace(name))
        {
            claims.Add(new Claim("name", name.Trim()));
        }

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static long Epoch(DateTime utc) =>
        new DateTimeOffset(utc).ToUnixTimeSeconds();
}
