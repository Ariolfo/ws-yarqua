using Microsoft.AspNetCore.Mvc;
using Hidrix.Application.Common.Interfaces;
using Hidrix.Application.DTOs;

namespace Hidrix.Api.Controllers;

/// <summary>
/// Health check del servicio y de la base de datos.
/// </summary>
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa el controlador.
    /// </summary>
    public HealthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Verifica estado del API y conectividad SQL Server.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthDto>> Get(CancellationToken cancellationToken)
    {
        var connected = false;
        try
        {
            connected = await _unitOfWork.CanConnectAsync(cancellationToken);
        }
        catch
        {
            connected = false;
        }

        return Ok(new HealthDto
        {
            Status = connected ? "ok" : "degraded",
            Database = connected ? "connected" : "unavailable",
        });
    }
}
