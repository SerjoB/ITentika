using ItentikaApp.Models;

namespace ItentikaApp.Utilities;

public interface IIncidentCreatedListener
{
    Task OnIncidentCreatedAsync(Incident incident, CancellationToken ct);
}