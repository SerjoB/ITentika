namespace ItentikaApp.Models;

public class Incident
{
    public Guid Id { get; set; }
    public IncidentType Type { get; set; }
    public DateTime Time { get; set; }
    public List<EventRecord> EventRecords { get; set; } = new();
    // ONLY USEFUL EVENTS ARE STORED
}