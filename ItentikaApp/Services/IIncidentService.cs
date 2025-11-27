using ItentikaApp.Models;

namespace ItentikaApp.Services;

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(
        IncidentType type,
        IEnumerable<Event> sourceEvents,
        CancellationToken ct = default);
}