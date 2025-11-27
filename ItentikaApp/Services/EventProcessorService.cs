using ItentikaApp.Models;
using ItentikaApp.Utilities.Patterns;

namespace ItentikaApp.Services;

public class EventProcessorService : IEventProcessor
{
    private readonly List<IEventPattern> _patterns;

    public EventProcessorService(
        PatternHandler1 p1,
        PatternHandler2 p2,
        PatternHandler3 p3)
    {
        _patterns = new() { p3, p2, p1 };
    }

    public async Task ProcessEventAsync(Event ev, CancellationToken ct)
    {
        foreach (var p in _patterns)
        {
            await p.TryProcessAsync(ev, ct);
        }
    }
}