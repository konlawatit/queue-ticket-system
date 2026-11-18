using QueueSystem.Application.Contracts;
using QueueSystem.Domain;

namespace QueueSystem.Application.Services;

/// <summary>The four use cases of the kiosk.</summary>
public interface IQueueService
{
    /// <summary>Issues the next ticket. Safe to call concurrently.</summary>
    /// <exception cref="QueueFullException">Z9 has already been issued in this round.</exception>
    Task<TicketResponse> IssueTicketAsync(CancellationToken cancellationToken = default);

    /// <returns>The ticket, or <see langword="null"/> if no ticket has this id.</returns>
    Task<TicketResponse?> GetTicketAsync(long id, CancellationToken cancellationToken = default);

    Task<CurrentQueueResponse> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts a new round. Returns "00".</summary>
    Task<CurrentQueueResponse> ResetAsync(CancellationToken cancellationToken = default);
}
