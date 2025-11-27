using System.Threading.Channels;
using ItentikaApp.Data;
using ItentikaApp.Models;
using ItentikaApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItentikaApp.Controllers;

[ApiController]
[Route("api/events")]
public class EventProcessorController : ControllerBase
{
    private readonly Channel<Event> _channel;

    public EventProcessorController(Channel<Event> channel)
    {
        _channel = channel;
    }
    
    //SENDS EVENT FOR PROCESSING
    [HttpPost("process")]
    public IActionResult Process([FromBody] Event ev)
    {
        if (ev == null) return BadRequest();

        if (ev.Id == Guid.Empty) ev.Id = Guid.NewGuid();
        if (ev.Time == default) ev.Time = DateTime.UtcNow;

        // RETURNS FALSE IF QUE IS FULL
        var written = _channel.Writer.TryWrite(ev);

        if (!written)
        {
            return StatusCode(503, "Queue is full");
        }

        return Accepted(new { ev.Id });
    }
}