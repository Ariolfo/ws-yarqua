using Mediator;
using Microsoft.AspNetCore.Mvc;
using Hidrix.Application.Common.Models;
using Hidrix.Application.DTOs;
using Hidrix.Application.Features.Auth.Commands.Login;
using Hidrix.Application.Features.Auth.Commands.Refresh;
using Hidrix.Application.Features.Auth.Commands.Register;

namespace Hidrix.Api.Controllers;

/// <summary>
/// Endpoints de autenticación (registro, login y refresh).
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Inicializa el controlador.</summary>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo usuario con email y contraseña. Se le asigna el rol User.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var data = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<AuthDto>.Ok(data, "Registro exitoso"));
    }

    /// <summary>
    /// Inicia sesión con email y contraseña. Devuelve tokens JWT con roles.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var data = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<AuthDto>.Ok(data, "Sesión iniciada"));
    }

    /// <summary>
    /// Renueva access y refresh tokens (incluye roles actualizados).
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<RefreshDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<RefreshDto>>> Refresh(
        [FromBody] RefreshCommand command,
        CancellationToken cancellationToken)
    {
        var data = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<RefreshDto>.Ok(data, "Tokens renovados"));
    }
}
