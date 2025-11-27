using ItentikaApp.Data;
using ItentikaApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItentikaApp.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentRepository _repository;

    public IncidentsController(IIncidentRepository repository)
    {
        _repository = repository;
    }

    // ENDPOINT FOR DISPLAYING INCIDENTS PAGINATED (ORDERED BY TIME DESC)
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken clt = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 200) pageSize = 20;

        var (items, total) = await _repository.GetPagedAsync(page, pageSize, clt);

        var result = new
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(i => new
            {
                i.Id,
                Type = (int)i.Type,
                i.Time,
                Events = i.EventRecords.Select(e => new { Type = (int)e.Type, e.Time })
            })
        };

        return Ok(result);
    }
}