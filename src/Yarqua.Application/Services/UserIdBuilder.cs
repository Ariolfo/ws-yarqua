using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Yarqua.Application.Services;

/// <summary>
/// Utilidades de identificadores de usuario y slug.
/// </summary>
public static class UserIdBuilder
{
    /// <summary>
    /// Construye el id lógico: u- + primeros 16 hex de SHA256(name|ciuId).
    /// </summary>
    public static string Build(string nombre, int ciuId)
    {
        var raw = $"{nombre.Trim()}|{ciuId}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return "u-" + hex[..16];
    }
}

/// <summary>
/// Generación de slugs estilo python-slugify.
/// </summary>
public static class SlugHelper
{
    /// <summary>
    /// Convierte texto a slug ASCII en minúsculas.
    /// </summary>
    /// <param name="text">Texto de entrada.</param>
    /// <param name="separator">Separador (por defecto "-").</param>
    /// <returns>Slug normalizado.</returns>
    public static string Slugify(string text, string separator = "-")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text.Trim().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(separator);
            }
        }

        var slug = Regex.Replace(sb.ToString(), $"{Regex.Escape(separator)}+", separator);
        return slug.Trim(separator.ToCharArray());
    }
}
