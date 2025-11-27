using ItentikaApp.Data;
using ItentikaApp.Models;
using ItentikaApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItentikaApp.Controllers;

[ApiController]
[Route("api/generator")]
public class EventGeneratorController : ControllerBase
{
    private readonly IEventGenerator _generator;
    private readonly IHttpClientFactory _httpFactory;

    public EventGeneratorController(IEventGenerator generator, IHttpClientFactory httpFactory)
    {
        _generator = generator;
        _httpFactory = httpFactory;
    }

    // ENDPOINT FOR GENERATING EVENTS IN SWAGGER
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromQuery] int? forceType = null)
    {
        var evt = _generator.GenerateRandomEvent();
        if (forceType.HasValue && forceType.Value >= 1 && forceType.Value <= 3)
        {
            evt.Type = (EventType)forceType.Value;
        }

        var client = _httpFactory.CreateClient("processor");
        // SENDS EVENT TO BE PROCESSED
        var resp = await client.PostAsJsonAsync("/api/events/process", evt);

        if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

        return Ok(evt);
    }
}