using ItentikaApp.Data;

namespace ItentikaApp.Models;

public class EventRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public EventType Type { get; set; }

    public DateTime Time { get; set; }

    public Guid IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
}