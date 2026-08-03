using FluentValidation;
using MediatR;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Application.Services;
using Yarqua.Domain.Entities;

namespace Yarqua.Application.Features.Auth.Commands.Register;

/// <summary>
/// Comando de registro de usuario.
/// </summary>
public class RegisterCommand : IRequest<AuthDto>
{
    /// <summary>Nombre del usuario.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>País (nombre o id).</summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>Departamento.</summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>Ciudad.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Id estable del dispositivo (opcional).</summary>
    public string? DeviceId { get; set; }

    /// <summary>Plataforma: android, ios, web.</summary>
    public string Platform { get; set; } = "android";
}

/// <summary>
/// Validador del comando de registro.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>
    /// Configura reglas de validación.
    /// </summary>
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(150);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(150);
        RuleFor(x => x.City).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DeviceId)
            .MinimumLength(8)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));
    }
}

/// <summary>
/// Handler de registro: resuelve geo, upsert usuario, evento REGISTRO y JWT.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthDto>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IEventoUsuarioRepository _eventos;
    private readonly IUsuarioDispositivoRepository _dispositivos;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoResolver _geoResolver;
    private readonly IJwtTokenService _jwt;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    public RegisterCommandHandler(
        IUsuarioRepository usuarios,
        IEventoUsuarioRepository eventos,
        IUsuarioDispositivoRepository dispositivos,
        IUnitOfWork unitOfWork,
        IGeoResolver geoResolver,
        IJwtTokenService jwt)
    {
        _usuarios = usuarios;
        _eventos = eventos;
        _dispositivos = dispositivos;
        _unitOfWork = unitOfWork;
        _geoResolver = geoResolver;
        _jwt = jwt;
    }

    /// <summary>
    /// Ejecuta el registro.
    /// </summary>
    public async Task<AuthDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ResolvedLocation location;
        try
        {
            location = await _geoResolver.ResolveAsync(
                request.Country,
                request.Department,
                request.City,
                cancellationToken);
        }
        catch (AppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new AppException(ex.Message);
        }

        var nombre = request.Name.Trim();
        var existing = await _usuarios.FindAsync(
            nombre,
            location.CodigoPais,
            location.CodigoDepartamento,
            location.CodigoCiudad,
            cancellationToken);

        if (existing is null)
        {
            _usuarios.Add(new YarqtbUsuario
            {
                UsuaNombre = nombre,
                UsuaCodigoPais = location.CodigoPais,
                UsuaCodigoDepartamento = location.CodigoDepartamento,
                UsuaCodigoCiudad = location.CodigoCiudad,
                UsuaActivo = true,
                UsuaFechaRegistro = DateTime.UtcNow,
                UsuaFechaCreacion = DateTime.UtcNow,
                UsuaFechaActualizacion = DateTime.UtcNow,
            });
        }
        else
        {
            existing.UsuaActivo = true;
            existing.UsuaFechaActualizacion = DateTime.UtcNow;
        }

        _eventos.Add(new YarqtbEventoUsuario
        {
            UsuaNombre = nombre,
            EvenEvento = "REGISTRO",
            EvenFecha = DateOnly.FromDateTime(DateTime.UtcNow),
            EvenHora = TimeOnly.FromDateTime(DateTime.UtcNow),
            EvenFechaCreacion = DateTime.UtcNow,
            EvenFechaActualizacion = DateTime.UtcNow,
        });

        var userId = UserIdBuilder.Build(
            nombre,
            location.CodigoPais,
            location.CodigoDepartamento,
            location.CodigoCiudad);

        if (!string.IsNullOrWhiteSpace(request.DeviceId))
        {
            var platform = string.IsNullOrWhiteSpace(request.Platform) ? "android" : request.Platform.Trim().ToLowerInvariant();
            if (platform is not ("android" or "ios" or "web" or "unknown"))
            {
                platform = "unknown";
            }

            var device = await _dispositivos.FindByDeviceIdAsync(request.DeviceId!, cancellationToken);

            if (device is null)
            {
                _dispositivos.Add(new YarqtbUsuarioDispositivo
                {
                    UdiDeviceId = request.DeviceId!,
                    UsuaId = userId,
                    UdiPlatform = platform,
                    UdiActivo = true,
                    UdiFechaRegistro = DateTime.UtcNow,
                    UdiFechaActualizacion = DateTime.UtcNow,
                });
            }
            else
            {
                device.UsuaId = userId;
                device.UdiPlatform = platform;
                device.UdiActivo = true;
                device.UdiFechaActualizacion = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto
        {
            AccessToken = _jwt.CreateAccessToken(userId, nombre),
            RefreshToken = _jwt.CreateRefreshToken(userId, nombre),
            User = new UserDto
            {
                Id = userId,
                Name = nombre,
                Country = location.PaisNombre,
                Department = location.DepartamentoNombre,
                City = location.CiudadNombre,
            },
        };
    }
}
