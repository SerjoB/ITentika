using ItentikaApp.Data;
using ItentikaApp.Models;

namespace ItentikaApp.Services;

public interface IEventProcessor
{
    Task ProcessEventAsync(Event ev, CancellationToken clt = default);
}