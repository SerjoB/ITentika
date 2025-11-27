using System.Collections.Concurrent;
using ItentikaApp.Models;
using ItentikaApp.Services;

namespace ItentikaApp.Utilities.Patterns;

public class PatternHandler3 : IEventPattern, IIncidentCreatedListener
{
    private record Pending(Event E3, CancellationTokenSource Cts);

    private readonly ConcurrentDictionary<Guid, Pending> _pending = new();
    private readonly IServiceScopeFactory _scopeFactory;

    public PatternHandler3(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory =  scopeFactory;
    }

    public Task<bool> TryProcessAsync(Event ev, CancellationToken ct)
    {
        if (ev.Type != EventType.Type3)
            return Task.FromResult(false);

        var cts = new CancellationTokenSource();
        _pending[ev.Id] = new Pending(ev, cts);

        _ = RunTimeout(ev.Id, ev, cts);

        return Task.FromResult(true);
    }

    private async Task RunTimeout(Guid id, Event e3, CancellationTokenSource cts)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(60), cts.Token);

            if (_pending.TryRemove(id, out _))
            {
                using var scope = _scopeFactory.CreateScope();
                var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();
                await incidentService.CreateIncidentAsync(
                    IncidentType.Type1,
                    new[] { e3 },
                    CancellationToken.None);
            }
        }
        catch (TaskCanceledException)
        {
            // EXPECTED CANCELLATION, NO FURTHER ACTIONS REQUIRED
        }
    }
    
    // CALLED FOR EVERY INCIDENT CREATED
    public async Task OnIncidentCreatedAsync(Incident incident, CancellationToken ct)
    {
        if (incident.Type != IncidentType.Type2)
            return;

        var match = _pending.Values
            .FirstOrDefault(p =>
                (incident.Time - p.E3.Time).TotalSeconds <= 60);

        if (match == null)
            return;

        if (_pending.TryRemove(match.E3.Id, out var entry))
        {
            entry.Cts.Cancel();
            using var scope = _scopeFactory.CreateScope();
            var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();
            var sourceEvents = incident.EventRecords.Select(e => 
                new Event { Id = e.Id, Type = e.Type, Time = e.Time}).Append(match.E3); 
            await incidentService.CreateIncidentAsync(
                IncidentType.Type3,
                sourceEvents,
                CancellationToken.None);
        }
    }
}
