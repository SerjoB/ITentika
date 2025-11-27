using System.Threading.Channels;
using ItentikaApp.Data;
using ItentikaApp.Models;
using ItentikaApp.Services;

namespace ItentikaApp.Background;

public class EventProcessorBackgroundService : BackgroundService
{
    private readonly Channel<Event> _channel;
    private readonly IServiceProvider _provider;
    private readonly ILogger<EventProcessorBackgroundService> _logger;

    public EventProcessorBackgroundService(Channel<Event> channel, IServiceProvider provider, ILogger<EventProcessorBackgroundService> logger)
    {
        _channel = channel;
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken clt)
    {
        _logger.LogInformation("EventProcessorBackgroundService started");

        while (!clt.IsCancellationRequested)
        {
            Event ev;
            try
            {
                ev = await _channel.Reader.ReadAsync(clt);  // RECEIVES EVENT FROM CHANNEL
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                using var scope = _provider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

                _logger.LogInformation("Processing event {EventId} type={Type}", ev.Id, ev.Type);
                await processor.ProcessEventAsync(ev, clt); // REAL PROCESSING HAPPENS HERE
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing event {EventId}", ev.Id);
            }
        }

        _logger.LogInformation("EventProcessorBackgroundService stopping");
    }
}