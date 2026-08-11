namespace Yarqua.Infrastructure.Identity;

/// <summary>
/// Constantes de roles de la aplicación.
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Operador = "Operador";
    public const string Visualizador = "Visualizador";

    public static readonly string[] All = [Admin, Operador, Visualizador];
}
