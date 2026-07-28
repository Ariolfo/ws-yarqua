using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Yarqua.Application.Common.Behaviors;

namespace Yarqua.Application;

/// <summary>
/// Registro de dependencias de la capa Application.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega MediatR, FluentValidation y behaviours de Application.
    /// </summary>
    /// <param name="services">Contenedor DI.</param>
    /// <returns>El mismo contenedor.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
