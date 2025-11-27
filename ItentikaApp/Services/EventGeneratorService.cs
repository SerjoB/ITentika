using ItentikaApp.Data;
using ItentikaApp.Models;

namespace ItentikaApp.Services;

public class EventGeneratorService: IEventGenerator
{
    private readonly Random _random = new();

    public Event GenerateRandomEvent()
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            Type = (EventType)_random.Next(1, 4),
            Time = DateTime.UtcNow
        };
    }
}