using FluentValidation;
using MediatR;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;
using Yarqua.Domain.Entities;

namespace Yarqua.Application.Features.Auth.Commands.Login;

/// <summary>
/// Comando de inicio de sesión con email y contraseña.
/// </summary>
public class LoginCommand : IRequest<AuthDto>
{
    /// <summary>Correo electrónico.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Contraseña.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Id estable del dispositivo (opcional).</summary>
    public string? DeviceId { get; set; }

    /// <summary>Plataforma: android, ios, web.</summary>
    public string Platform { get; set; } = "android";
}

/// <summary>
/// Validador del comando de login.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Configura reglas de validación.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(1);
        RuleFor(x => x.DeviceId)
            .MinimumLength(8).MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.DeviceId));
    }
}

/// <summary>
/// Handler de login: valida credenciales, registra evento ACCESO y emite JWT con roles.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
{
    private readonly IIdentityService _identity;
    private readonly IEventoUsuarioRepository _eventos;
    private readonly IUsuarioDispositivoRepository _dispositivos;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwt;

    /// <summary>Inicializa el handler.</summary>
    public LoginCommandHandler(
        IIdentityService identity,
        IEventoUsuarioRepository eventos,
        IUsuarioDispositivoRepository dispositivos,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwt)
    {
        _identity = identity;
        _eventos = eventos;
        _dispositivos = dispositivos;
        _unitOfWork = unitOfWork;
        _jwt = jwt;
    }

    /// <summary>Ejecuta el login.</summary>
    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identity.LoginAsync(
            request.Email.Trim().ToLowerInvariant(),
            request.Password,
            cancellationToken);

        if (!result.Success)
            throw new UnauthorizedAppException("Credenciales inválidas o cuenta bloqueada.");

        _eventos.Add(new YarqtbEventoUsuario
        {
            UsuaNombre = result.DisplayName,
            EvenEvento = "ACCESO",
            EvenFecha = DateOnly.FromDateTime(DateTime.UtcNow),
            EvenHora = TimeOnly.FromDateTime(DateTime.UtcNow),
            EvenFechaCreacion = DateTime.UtcNow,
            EvenFechaActualizacion = DateTime.UtcNow,
        });

        if (!string.IsNullOrWhiteSpace(request.DeviceId))
        {
            var platform = NormalizePlatform(request.Platform);
            var device = await _dispositivos.FindByDeviceIdAsync(request.DeviceId!, cancellationToken);
            if (device is null)
            {
                _dispositivos.Add(new YarqtbUsuarioDispositivo
                {
                    UdiDeviceId = request.DeviceId!,
                    UsuaId = result.UserId,
                    UdiPlatform = platform,
                    UdiActivo = true,
                    UdiFechaRegistro = DateTime.UtcNow,
                    UdiFechaActualizacion = DateTime.UtcNow,
                });
            }
            else
            {
                device.UsuaId = result.UserId;
                device.UdiPlatform = platform;
                device.UdiActivo = true;
                device.UdiFechaActualizacion = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthDto
        {
            AccessToken = _jwt.CreateAccessToken(result.UserId, result.DisplayName, result.Roles),
            RefreshToken = _jwt.CreateRefreshToken(result.UserId, result.DisplayName),
            User = new UserDto
            {
                Id = result.UserId,
                Name = result.DisplayName,
                Email = result.Email,
                Roles = result.Roles,
            },
        };
    }

    private static string NormalizePlatform(string platform)
    {
        var p = platform.Trim().ToLowerInvariant();
        return p is "android" or "ios" or "web" or "unknown" ? p : "unknown";
    }
}
