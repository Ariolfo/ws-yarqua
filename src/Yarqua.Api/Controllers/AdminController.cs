using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.Common.Models;
using Yarqua.Infrastructure.Identity;
using Yarqua.Infrastructure.Persistence;

namespace Yarqua.Api.Controllers;

/// <summary>
/// Endpoints de administración de usuarios, roles y grupos (solo Admin).
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityService _identity;
    private readonly YarquaDbContext _db;

    /// <summary>Inicializa el controlador.</summary>
    public AdminController(UserManager<ApplicationUser> userManager, IIdentityService identity, YarquaDbContext db)
    {
        _userManager = userManager;
        _identity = identity;
        _db = db;
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

    /// <summary>Lista los grupos existentes.</summary>
    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
    {
        var groups = await _db.Grupos
            .Where(g => g.GrupActivo)
            .Select(g => new { g.GrupId, g.GrupNombre, g.GrupDescripcion, g.GrupRol, g.GrupFechaCreacion })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(groups, "Grupos listados"));
    }

    /// <summary>Crea un grupo nuevo.</summary>
    [HttpPost("groups")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        if (!AppRoles.All.Contains(request.Rol))
            return BadRequest(ApiResponse<object>.Fail($"Rol inválido. Roles válidos: {string.Join(", ", AppRoles.All)}"));

        var group = new Yarqua.Domain.Entities.YarqtbGrupo
        {
            GrupNombre = request.Nombre.Trim(),
            GrupDescripcion = request.Descripcion?.Trim(),
            GrupRol = request.Rol,
            GrupActivo = true,
            GrupFechaCreacion = DateTime.UtcNow,
        };

        _db.Grupos.Add(group);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { group.GrupId, group.GrupNombre, group.GrupRol }, "Grupo creado"));
    }

    /// <summary>Agrega un usuario a un grupo y le asigna el rol del grupo.</summary>
    [HttpPost("groups/{groupId}/members/{userId}")]
    public async Task<IActionResult> AddMember(int groupId, string userId, CancellationToken cancellationToken)
    {
        var group = await _db.Grupos.FindAsync([groupId], cancellationToken);
        if (group is null || !group.GrupActivo)
            return NotFound(ApiResponse<object>.Fail("Grupo no encontrado"));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("Usuario no encontrado"));

        var alreadyMember = await _db.UsuarioGrupos
            .AnyAsync(ug => ug.UsuaId == userId && ug.GrupId == groupId, cancellationToken);

        if (!alreadyMember)
        {
            _db.UsuarioGrupos.Add(new Yarqua.Domain.Entities.YarqtbUsuarioGrupo
            {
                UsuaId = userId,
                GrupId = groupId,
                UgrFechaAsignacion = DateTime.UtcNow,
            });

            if (!await _userManager.IsInRoleAsync(user, group.GrupRol))
                await _userManager.AddToRoleAsync(user, group.GrupRol);

            await _db.SaveChangesAsync(cancellationToken);
        }

        return Ok(ApiResponse<object>.Ok(null, "Usuario agregado al grupo"));
    }
}

/// <summary>Request para crear un grupo.</summary>
public class CreateGroupRequest
{
    /// <summary>Nombre del grupo.</summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Descripción opcional.</summary>
    public string? Descripcion { get; set; }
    /// <summary>Rol asociado (Admin | Operador | Visualizador).</summary>
    public string Rol { get; set; } = string.Empty;
}
