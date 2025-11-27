using ItentikaApp.Data;
using ItentikaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ItentikaApp.Repositories;

// FOR CRUD AND BASIC OPERATIONS WITH DB
public class IncidentRepository: IIncidentRepository
{
    private readonly AppDbContext _context;

    public IncidentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddIncidentAsync(Incident incident, CancellationToken ct = default)
    {
        await _context.Incidents.AddAsync(incident, ct);
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<Incident>> GetAllIncidentsAsync(CancellationToken ct = default)
    {
        return _context.Incidents
            .Include(i => i.EventRecords)
            .ToListAsync(ct);
    }
    
     public async Task<(IEnumerable<Incident> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Incidents.Include(i => i.EventRecords).AsNoTracking().OrderByDescending(i => i.Time);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public Task<Incident?> GetIncidentByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _context.Incidents
            .Include(i => i.EventRecords)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }
}