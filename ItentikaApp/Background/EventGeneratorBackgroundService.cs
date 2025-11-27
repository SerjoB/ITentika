using ItentikaApp.Services;

namespace ItentikaApp.Background;

public class EventGeneratorBackgroundService : BackgroundService
{
    private readonly IEventGenerator _generator;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<EventGeneratorBackgroundService> _logger;
    private readonly Random _random = new();

    public EventGeneratorBackgroundService(IEventGenerator generator, IHttpClientFactory httpFactory, ILogger<EventGeneratorBackgroundService> logger)
    {
        _generator = generator;
        _httpFactory = httpFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _logger.LogInformation("EventGeneratorBackgroundService started");

        while (!clt.IsCancellationRequested)
        {
            // WAITING INTERVAL
            var waitMs = _random.Next(0, 2000);
            await Task.Delay(waitMs, clt);

            var evt = _generator.GenerateRandomEvent();
            
            try
            {
                // HTTP CLIENT SET UP IN DI CONTAINER
                var client = _httpFactory.CreateClient("processor");

                var resp = await client.PostAsJsonAsync("/api/events/process", evt, clt);

                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Processor returned {Status} for event {EventId}", resp.StatusCode, evt.Id);
                }
                else
                {
                    _logger.LogInformation("Generated and sent event {EventId} type={Type}", evt.Id, evt.Type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event {EventId}", evt.Id);
            }
        }

        _logger.LogInformation("EventGeneratorBackgroundService stopping");
    }
}