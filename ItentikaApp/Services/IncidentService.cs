using ItentikaApp.Models;
using ItentikaApp.Repositories;
using ItentikaApp.Utilities;

namespace ItentikaApp.Services;

public class IncidentService : IIncidentService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<IIncidentCreatedListener> _listeners;

    public IncidentService(
        IServiceScopeFactory scopeFactory,
        IEnumerable<IIncidentCreatedListener> listeners)
    {
        _scopeFactory = scopeFactory;
        _listeners = listeners;
    }

    public async Task<Incident> CreateIncidentAsync(
        IncidentType type,
        IEnumerable<Event> sourceEvents,
        CancellationToken ct = default)
    {
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            Time = DateTime.UtcNow,
            Type = type,
            EventRecords = sourceEvents.Select(e => new EventRecord
            {
                Id = Guid.NewGuid(),
                Type = e.Type,
                Time = e.Time
            }).ToList()
        };

        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IIncidentRepository>();

        await repo.AddIncidentAsync(incident, ct);

        // NOTIFYING PATTERNS
        foreach (var listener in _listeners)
        {
            await listener.OnIncidentCreatedAsync(incident, ct);
        }

        return incident;
    }
}
