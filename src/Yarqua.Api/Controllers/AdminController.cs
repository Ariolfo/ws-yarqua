using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.Common.Models;
using Yarqua.Infrastructure.Identity;

namespace Yarqua.Api.Controllers;

/// <summary>
/// Endpoints de administración de usuarios y roles (solo Admin).
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityService _identity;

    /// <summary>Inicializa el controlador.</summary>
    public AdminController(UserManager<ApplicationUser> userManager, IIdentityService identity)
    {
        _userManager = userManager;
        _identity = identity;
    }

    /// <summary>Lista todos los usuarios con sus roles.</summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .Select(u => new { u.Id, u.UsuaNombre, u.Email, u.UsuaActivo, u.UsuaFechaRegistro })
            .ToListAsync(cancellationToken);

        var result = new List<object>();
        foreach (var u in users)
        {
            var user = await _userManager.FindByIdAsync(u.Id);
            var roles = user is not null ? await _userManager.GetRolesAsync(user) : [];
            result.Add(new { u.Id, u.UsuaNombre, u.Email, u.UsuaActivo, u.UsuaFechaRegistro, Roles = roles });
        }

        return Ok(ApiResponse<object>.Ok(result, "Usuarios listados"));
    }

    /// <summary>Asigna un rol a un usuario.</summary>
    [HttpPost("users/{userId}/roles/{role}")]
    public async Task<IActionResult> AssignRole(string userId, string role, CancellationToken cancellationToken)
    {
        if (!AppRoles.All.Contains(role))
            return BadRequest(ApiResponse<object>.Fail($"Rol inválido. Roles válidos: {string.Join(", ", AppRoles.All)}"));

        var errors = await _identity.AssignRoleAsync(userId, role, cancellationToken);
        if (errors.Length > 0)
            return BadRequest(ApiResponse<object>.Fail(string.Join("; ", errors)));

        return Ok(ApiResponse<object>.Ok(null, $"Rol '{role}' asignado correctamente"));
    }

    /// <summary>Quita un rol a un usuario.</summary>
    [HttpDelete("users/{userId}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(string userId, string role, CancellationToken cancellationToken)
    {
        var success = await _identity.RemoveRoleAsync(userId, role, cancellationToken);
        if (!success)
            return NotFound(ApiResponse<object>.Fail("Usuario no encontrado o no tenía ese rol"));

        return Ok(ApiResponse<object>.Ok(null, $"Rol '{role}' eliminado correctamente"));
    }
}
