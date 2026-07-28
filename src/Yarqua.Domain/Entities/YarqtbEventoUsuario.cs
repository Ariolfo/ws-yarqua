namespace Yarqua.Domain.Entities;

/// <summary>
/// Evento de auditoría de usuario (REGISTRO, ACCESO, CONSULTA).
/// </summary>
public class YarqtbEventoUsuario
{
    /// <summary>Identificador del evento.</summary>
    public long EvenId { get; set; }

    /// <summary>Nombre del usuario asociado.</summary>
    public string UsuaNombre { get; set; } = string.Empty;

    /// <summary>Fecha del evento.</summary>
    public DateOnly EvenFecha { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>Hora del evento.</summary>
    public TimeOnly EvenHora { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>Tipo de evento (REGISTRO, ACCESO, CONSULTA).</summary>
    public string EvenEvento { get; set; } = string.Empty;

    /// <summary>Sensor lógico consultado (opcional).</summary>
    public string? EvenSensorId { get; set; }

    /// <summary>Fecha de creación.</summary>
    public DateTimeOffset EvenFechaCreacion { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Fecha de actualización.</summary>
    public DateTimeOffset EvenFechaActualizacion { get; set; } = DateTimeOffset.UtcNow;
}
