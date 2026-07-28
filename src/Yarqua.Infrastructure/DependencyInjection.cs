using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarqua.Application.Common.Interfaces;
using Yarqua.Infrastructure.Options;
using Yarqua.Infrastructure.Persistence;
using Yarqua.Infrastructure.Persistence.Repositories;
using Yarqua.Infrastructure.Services;

namespace Yarqua.Infrastructure;

/// <summary>
/// Registro de dependencias de Infrastructure.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega EF Core, JWT, HttpClient Visualiti y servicios de infraestructura.
    /// </summary>
    /// <param name="services">Contenedor DI.</param>
    /// <param name="configuration">Configuración.</param>
    /// <returns>El mismo contenedor.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<VisualitiOptions>(configuration.GetSection(VisualitiOptions.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection no configurada");

        services.AddDbContext<YarquaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEventoUsuarioRepository, EventoUsuarioRepository>();
        services.AddScoped<IUsuarioDispositivoRepository, UsuarioDispositivoRepository>();
        services.AddScoped<IGeoRepository, GeoRepository>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IGeoResolver, GeoResolver>();
        services.AddSingleton<IVisualitiMoistureCache, VisualitiMoistureCache>();

        var visualiti = configuration.GetSection(VisualitiOptions.SectionName).Get<VisualitiOptions>()
                        ?? new VisualitiOptions();

        services.AddHttpClient<IVisualitiClient, VisualitiClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (!visualiti.SslVerify)
                {
                    handler.ServerCertificateCustomValidationCallback =
                        static (HttpRequestMessage _, X509Certificate2? _, X509Chain? _, SslPolicyErrors _) => true;
                }

                return handler;
            });

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            string.IsNullOrWhiteSpace(jwt.Secret)
                ? "Yarqua_Dev_Secret_Key_Change_Me_32chars!"
                : jwt.Secret));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();
        return services;
    }
}
