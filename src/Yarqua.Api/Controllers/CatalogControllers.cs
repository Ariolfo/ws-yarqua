using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.Common.Models;
using Yarqua.Application.DTOs;

namespace Yarqua.Api.Controllers;

/// <summary>
/// Catálogo de cultivos (capacidad de campo / decisión de riego).
/// </summary>
[ApiController]
[Route("api/v1/crops")]
[Authorize]
public class CropsController : ControllerBase
{
    private readonly ICropCatalogService _crops;

    /// <summary>Inicializa el controlador.</summary>
    public CropsController(ICropCatalogService crops)
    {
        _crops = crops;
    }

    /// <summary>Lista cultivos activos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CropDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CropDto>>>> List(CancellationToken cancellationToken)
    {
        var data = await _crops.ListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CropDto>>.Ok(data));
    }

    /// <summary>Obtiene un cultivo por id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CropDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CropDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var data = await _crops.GetByIdAsync(id, cancellationToken);
        if (data is null)
        {
            return NotFound(ApiResponse<CropDto>.Fail("Cultivo no encontrado"));
        }

        return Ok(ApiResponse<CropDto>.Ok(data));
    }

    /// <summary>Crea un cultivo manualmente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CropDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CropDto>>> Create(
        [FromBody] CreateCropRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _crops.CreateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<CropDto>.Ok(data, "Cultivo creado"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<CropDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<CropDto>.Fail(ex.Message));
        }
    }

    /// <summary>Actualiza un cultivo existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CropDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CropDto>>> Update(
        int id,
        [FromBody] CreateCropRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _crops.UpdateAsync(id, request, cancellationToken);
            return Ok(ApiResponse<CropDto>.Ok(data, "Cultivo actualizado"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CropDto>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<CropDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<CropDto>.Fail(ex.Message));
        }
    }
}

/// <summary>
/// Catálogo de sensores por país/red (CRUD ligero).
/// </summary>
[ApiController]
[Route("api/v1/catalog/sensors")]
[Authorize]
public class CatalogSensorsController : ControllerBase
{
    private readonly ISensorCatalogService _sensors;

    /// <summary>Inicializa el controlador.</summary>
    public CatalogSensorsController(ISensorCatalogService sensors)
    {
        _sensors = sensors;
    }

    /// <summary>Lista sensores del catálogo.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CatalogSensorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CatalogSensorDto>>>> List(
        CancellationToken cancellationToken)
    {
        var data = await _sensors.ListCatalogAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CatalogSensorDto>>.Ok(data));
    }

    /// <summary>Lista redes con país.</summary>
    [HttpGet("~/api/v1/catalog/networks")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NetworkDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NetworkDto>>>> ListNetworks(
        CancellationToken cancellationToken)
    {
        var data = await _sensors.ListNetworksAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<NetworkDto>>.Ok(data));
    }

    /// <summary>Obtiene un sensor del catálogo por id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CatalogSensorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CatalogSensorDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await _sensors.GetCatalogByIdAsync(id, cancellationToken);
        if (data is null)
        {
            return NotFound(ApiResponse<CatalogSensorDto>.Fail("Sensor no encontrado"));
        }

        return Ok(ApiResponse<CatalogSensorDto>.Ok(data));
    }

    /// <summary>Crea un sensor manualmente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CatalogSensorDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CatalogSensorDto>>> Create(
        [FromBody] CreateCatalogSensorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _sensors.CreateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<CatalogSensorDto>.Ok(data, "Sensor creado"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<CatalogSensorDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<CatalogSensorDto>.Fail(ex.Message));
        }
    }

    /// <summary>Actualiza un sensor del catálogo.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CatalogSensorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CatalogSensorDto>>> Update(
        int id,
        [FromBody] CreateCatalogSensorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _sensors.UpdateAsync(id, request, cancellationToken);
            return Ok(ApiResponse<CatalogSensorDto>.Ok(data, "Sensor actualizado"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CatalogSensorDto>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<CatalogSensorDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<CatalogSensorDto>.Fail(ex.Message));
        }
    }
}

/// <summary>
/// Catálogo de métodos para capacidad de campo.
/// </summary>
[ApiController]
[Route("api/v1/catalog/metodos-cc")]
[Authorize]
public class MetodosCCController : ControllerBase
{
    private readonly IMetodoCCCatalogService _metodos;

    /// <summary>Inicializa el controlador.</summary>
    public MetodosCCController(IMetodoCCCatalogService metodos)
    {
        _metodos = metodos;
    }

    /// <summary>Lista métodos activos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MetodoCCDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MetodoCCDto>>>> List(
        CancellationToken cancellationToken)
    {
        var data = await _metodos.ListAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MetodoCCDto>>.Ok(data));
    }

    /// <summary>Obtiene un método por id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MetodoCCDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MetodoCCDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var data = await _metodos.GetByIdAsync(id, cancellationToken);
        if (data is null)
        {
            return NotFound(ApiResponse<MetodoCCDto>.Fail("Método no encontrado"));
        }

        return Ok(ApiResponse<MetodoCCDto>.Ok(data));
    }

    /// <summary>Crea un método.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MetodoCCDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MetodoCCDto>>> Create(
        [FromBody] CreateMetodoCCRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _metodos.CreateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<MetodoCCDto>.Ok(data, "Método creado"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<MetodoCCDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<MetodoCCDto>.Fail(ex.Message));
        }
    }

    /// <summary>Actualiza un método.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MetodoCCDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MetodoCCDto>>> Update(
        int id,
        [FromBody] CreateMetodoCCRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _metodos.UpdateAsync(id, request, cancellationToken);
            return Ok(ApiResponse<MetodoCCDto>.Ok(data, "Método actualizado"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MetodoCCDto>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<MetodoCCDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<MetodoCCDto>.Fail(ex.Message));
        }
    }
}
