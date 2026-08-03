using System.Text.Json.Serialization;
using DotNetEnv;
using Serilog;
using Yarqua.Api.Middleware;
using Yarqua.Application;
using Yarqua.Infrastructure;

// Carga opcional de ws-yarqua/.env (si existe) antes del host.
TryLoadDotEnv();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Yarqua.Api"));

    EnsureRequiredSecrets(builder.Configuration);

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Yarqua API", Version = "v1" });
    });

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "anonymous");
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La API Yarqua no pudo iniciar");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Busca y carga un archivo .env desde el cwd o la raíz de ws-yarqua.
static void TryLoadDotEnv()
{
    var candidates = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env")),
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env")),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env")),
    };

    foreach (var path in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
    {
        if (!File.Exists(path))
        {
            continue;
        }

        Env.Load(path);
        return;
    }
}

// Valida secretos mínimos (User Secrets, .env o variables de entorno).
static void EnsureRequiredSecrets(IConfiguration configuration)
{
    var missing = new List<string>();

    if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
    {
        missing.Add("ConnectionStrings:DefaultConnection");
    }

    if (string.IsNullOrWhiteSpace(configuration["Jwt:Secret"]))
    {
        missing.Add("Jwt:Secret");
    }

    if (string.Equals(configuration["Visualiti:Enabled"], "true", StringComparison.OrdinalIgnoreCase)
        || configuration.GetValue("Visualiti:Enabled", false))
    {
        if (string.IsNullOrWhiteSpace(configuration["Visualiti:Usuario"]))
        {
            missing.Add("Visualiti:Usuario");
        }

        if (string.IsNullOrWhiteSpace(configuration["Visualiti:Password"]))
        {
            missing.Add("Visualiti:Password");
        }
    }

    if (missing.Count == 0)
    {
        return;
    }

    throw new InvalidOperationException(
        "Faltan secretos de configuración: " + string.Join(", ", missing) +
        ". Use User Secrets (Development), variables de entorno o copie ws-yarqua/.env.example → .env");
}

/// <summary>
/// Punto de entrada parcial para tests de integración.
/// </summary>
public partial class Program;
