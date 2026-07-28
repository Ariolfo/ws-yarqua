using FluentValidation;
using MediatR;
using Yarqua.Application.Common.Exceptions;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Application.DTOs;

namespace Yarqua.Application.Features.Auth.Commands.Refresh;

/// <summary>
/// Comando de refresco de tokens JWT.
/// </summary>
public class RefreshCommand : IRequest<RefreshDto>
{
    /// <summary>Token de refresco.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Validador del comando de refresh.
/// </summary>
public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    /// <summary>
    /// Configura reglas de validación.
    /// </summary>
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MinimumLength(10);
    }
}

/// <summary>
/// Handler que emite nuevos access/refresh a partir de un refresh válido.
/// </summary>
public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshDto>
{
    private readonly IJwtTokenService _jwt;

    /// <summary>
    /// Inicializa el handler.
    /// </summary>
    /// <param name="jwt">Servicio JWT.</param>
    public RefreshCommandHandler(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    /// <summary>
    /// Ejecuta el refresco.
    /// </summary>
    /// <param name="request">Token de refresco.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Nuevos tokens.</returns>
    public Task<RefreshDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (sub, name) = _jwt.ValidateToken(request.RefreshToken, "refresh");
            return Task.FromResult(new RefreshDto
            {
                AccessToken = _jwt.CreateAccessToken(sub, name),
                RefreshToken = _jwt.CreateRefreshToken(sub, name),
            });
        }
        catch
        {
            throw new UnauthorizedAppException("Sesión expirada, vuelve a registrarte.");
        }
    }
}
