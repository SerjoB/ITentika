using System.Collections.Concurrent;
using ItentikaApp.Models;
using ItentikaApp.Services;

namespace ItentikaApp.Utilities.Patterns;

public class PatternHandler2 : IEventPattern
{
    private record Pending(Event E2, CancellationTokenSource Cts);

    private readonly ConcurrentDictionary<Guid, Pending> _pending = new();
    // private readonly IIncidentService _incidentService;
    private readonly IServiceScopeFactory _scopeFactory;

    public PatternHandler2(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<bool> TryProcessAsync(Event ev, CancellationToken ct)
    {
        switch (ev.Type)
        {
            case EventType.Type2:
                return await HandleType2(ev);
            case EventType.Type1:
                return await HandleType1(ev);
            default:
                return false;
        }
    }

    private async Task<bool> HandleType2(Event e2)
    {
        var cts = new CancellationTokenSource();
        _pending[e2.Id] = new Pending(e2, cts);

        _ = RunTimeout(e2.Id, e2, cts);

        return true;
    }

    private async Task RunTimeout(Guid id, Event e2, CancellationTokenSource cts)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20), cts.Token);

            if (_pending.TryRemove(id, out _))
            {
                using var scope = _scopeFactory.CreateScope();
                var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();
                await incidentService.CreateIncidentAsync(
                    IncidentType.Type1,
                    new[] { e2 },
                    CancellationToken.None);
            }
        }
        catch (TaskCanceledException)
        {
            // EXPECTED CANCELLATION, NO FURTHER ACTIONS REQUIRED
        }
    }

    private async Task<bool> HandleType1(Event e1)
    {
        var match = _pending.Values
            .FirstOrDefault(p => (e1.Time - p.E2.Time).TotalSeconds <= 20);

        if (match == null)
            return false;

        if (_pending.TryRemove(match.E2.Id, out var entry))
        {
            entry.Cts.Cancel();

            using var scope = _scopeFactory.CreateScope();
            var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();
            await incidentService.CreateIncidentAsync(
                IncidentType.Type2,
                new[] { entry.E2, e1 },
                CancellationToken.None);
        }

        return true;
    }
}
