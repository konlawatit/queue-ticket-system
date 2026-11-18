using QueueSystem.Domain;

namespace QueueSystem.Application.Abstractions;

/// <summary>
/// Port for loading and storing the queue. Implemented in the infrastructure layer.
/// </summary>
public interface IQueueRepository
{
    /// <summary>
    /// Loads the counter row and locks it until the current transaction ends,
    /// so a concurrent caller waits here instead of reading the same value.
    /// Call it only inside <see cref="IUnitOfWork.ExecuteInTransactionAsync{T}"/>,
    /// otherwise the lock is released as soon as the read finishes.
    /// </summary>
    Task<QueueState> GetStateForUpdateAsync(CancellationToken cancellationToken);

    /// <summary>Reads the counter row without locking it. For display only.</summary>
    Task<QueueState> GetStateAsync(CancellationToken cancellationToken);

    Task<Ticket?> FindTicketAsync(long id, CancellationToken cancellationToken);

    /// <summary>Marks a new ticket for insert; it is written when the unit of work saves.</summary>
    void AddTicket(Ticket ticket);
}
