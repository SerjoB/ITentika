using ItentikaApp.Models;

namespace ItentikaApp.Utilities.Patterns;

public interface IEventPattern
{
    Task<bool> TryProcessAsync(Event ev, CancellationToken ct);
}