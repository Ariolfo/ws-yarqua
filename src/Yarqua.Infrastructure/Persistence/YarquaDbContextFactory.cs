using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Yarqua.Infrastructure.Persistence;

/// <summary>
/// Factory de diseño para EF Core CLI (migrations add / database update).
/// Permite ejecutar comandos EF sin arrancar Program.cs ni necesitar la BD activa.
/// Lee la cadena de conexión de la variable de entorno YARQUA_CONNECTION_STRING
/// o usa una cadena local por defecto.
/// </summary>
public class YarquaDbContextFactory : IDesignTimeDbContextFactory<YarquaDbContext>
{
    /// <inheritdoc />
    public YarquaDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("YARQUA_CONNECTION_STRING")
            ?? "Server=localhost,1433;Database=dbYarqua;User Id=sa;Password=Yarqua_Str0ng!Passw0rd;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<YarquaDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new YarquaDbContext(options);
    }
}
