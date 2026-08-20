using Microsoft.AspNetCore.Identity;
using Hidrix.Application.Common.Interfaces;
using Hidrix.Infrastructure.Identity;

namespace Hidrix.Infrastructure.Services;

/// <summary>
/// Implementación de IIdentityService usando ASP.NET Core Identity.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>Inicializa el servicio.</summary>
    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<RegisterResult> RegisterAsync(
        string email,
        string password,
        string displayName,
        int? ciuId,
        CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            UsuaNombre = displayName,
            CiuId = ciuId,
            UsuaActivo = true,
            UsuaFechaRegistro = DateTime.UtcNow,
            UsuaFechaCreacion = DateTime.UtcNow,
            UsuaFechaActualizacion = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return new RegisterResult(false, string.Empty, result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, AppRoles.User);
        return new RegisterResult(true, user.Id, []);
    }

    /// <inheritdoc />
    public async Task<LoginResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.UsuaActivo)
            return new LoginResult(false, string.Empty, string.Empty, string.Empty, [], ["Credenciales inválidas."]);

        if (await _userManager.IsLockedOutAsync(user))
            return new LoginResult(false, string.Empty, string.Empty, string.Empty, [], ["Cuenta bloqueada temporalmente."]);

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            return new LoginResult(false, string.Empty, string.Empty, string.Empty, [], ["Credenciales inválidas."]);
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        var roles = (await _userManager.GetRolesAsync(user)).ToArray();
        return new LoginResult(true, user.Id, user.UsuaNombre, user.Email ?? string.Empty, roles, []);
    }

    /// <inheritdoc />
    public async Task<string[]> GetUserRolesAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return [];
        return (await _userManager.GetRolesAsync(user)).ToArray();
    }

    /// <inheritdoc />
    public async Task<string[]> AssignRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return ["Usuario no encontrado."];

        if (!await _userManager.IsInRoleAsync(user, role))
            await _userManager.AddToRoleAsync(user, role);

        return [];
    }

    /// <inheritdoc />
    public async Task<bool> RemoveRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<(string? DisplayName, string? Email)> GetUserInfoAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return (user?.UsuaNombre, user?.Email);
    }
}
