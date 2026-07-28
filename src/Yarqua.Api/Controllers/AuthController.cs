using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yarqua.Application.Common.Models;
using Yarqua.Application.DTOs;
using Yarqua.Application.Features.Auth.Commands.Refresh;
using Yarqua.Application.Features.Auth.Commands.Register;

namespace Yarqua.Api.Controllers;

/// <summary>
/// Endpoints de autenticación (registro y refresh).
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa el controlador.
    /// </summary>
    /// <param name="mediator">MediatR.</param>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra o actualiza un usuario y emite tokens JWT.
    /// </summary>
    /// <param name="command">Datos de registro.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sobre con AuthDto.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var data = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<AuthDto>.Ok(data, "Registro exitoso"));
    }

    /// <summary>
    /// Renueva access y refresh tokens.
    /// </summary>
    /// <param name="command">Token de refresco.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sobre con RefreshDto.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<RefreshDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RefreshDto>>> Refresh(
        [FromBody] RefreshCommand command,
        CancellationToken cancellationToken)
    {
        var data = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<RefreshDto>.Ok(data, "Tokens renovados"));
    }
}
