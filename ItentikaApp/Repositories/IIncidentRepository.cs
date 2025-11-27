using ItentikaApp.Data;
using ItentikaApp.Models;

namespace ItentikaApp.Repositories;

public interface IIncidentRepository
{
    Task AddIncidentAsync(Incident incident, CancellationToken ct = default);
    Task<List<Incident>> GetAllIncidentsAsync(CancellationToken ct = default);

    Task<(IEnumerable<Incident> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Incident?> GetIncidentByIdAsync(Guid id, CancellationToken ct = default);
}