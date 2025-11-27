using ItentikaApp.Models;
using ItentikaApp.Services;

namespace ItentikaApp.Utilities.Patterns;

public class PatternHandler1 : IEventPattern
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PatternHandler1(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<bool> TryProcessAsync(Event ev, CancellationToken ct)
    {
        if (ev.Type != EventType.Type1)
            return false;

        using var scope = _scopeFactory.CreateScope();
        var incidentService = scope.ServiceProvider.GetRequiredService<IIncidentService>();
        await incidentService.CreateIncidentAsync(
            IncidentType.Type1,
            new[] { ev },
            ct);

        return true;
    }
}
