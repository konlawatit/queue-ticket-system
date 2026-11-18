using Microsoft.EntityFrameworkCore;
using QueueSystem.Application.Abstractions;
using QueueSystem.Domain;

namespace QueueSystem.Infrastructure.Persistence;

internal sealed class QueueRepository(QueueDbContext db) : IQueueRepository
{
    public Task<QueueState> GetStateForUpdateAsync(CancellationToken cancellationToken) =>
        // UPDLOCK: update locks are incompatible with each other, so a second caller
        // waits on this read until the first transaction commits, then reads the new value.
        // Readers that take only shared locks (GetStateAsync) are not blocked by it.
        // ROWLOCK: lock just this row. The value is sent as a parameter, not concatenated.
        db.QueueStates
            .FromSql($"SELECT * FROM [QueueStates] WITH (UPDLOCK, ROWLOCK) WHERE [Id] = {QueueState.SingletonId}")
            .SingleAsync(cancellationToken);

    public Task<QueueState> GetStateAsync(CancellationToken cancellationToken) =>
        db.QueueStates
            .AsNoTracking()
            .SingleAsync(s => s.Id == QueueState.SingletonId, cancellationToken);

    public Task<Ticket?> FindTicketAsync(long id, CancellationToken cancellationToken) =>
        db.Tickets
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

    public void AddTicket(Ticket ticket) => db.Tickets.Add(ticket);
}
