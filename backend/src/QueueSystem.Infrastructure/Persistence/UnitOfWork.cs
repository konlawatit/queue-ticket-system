using Microsoft.EntityFrameworkCore;
using QueueSystem.Application.Abstractions;

namespace QueueSystem.Infrastructure.Persistence;

/// <remarks>
/// No retrying execution strategy (EnableRetryOnFailure) on purpose: issuing a ticket
/// is not idempotent, so if the acknowledgement of a successful COMMIT were lost, a
/// retry would issue a second ticket. A failed request surfaces as an error instead,
/// and the user can press the button again.
/// </remarks>
internal sealed class UnitOfWork(QueueDbContext db) : IUnitOfWork
{
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken cancellationToken)
    {
        // READ COMMITTED (the default) is enough: the UPDLOCK taken inside the
        // work is held until this transaction ends, whatever the isolation level.
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var result = await work(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // If anything above throws, the transaction is disposed without a commit and rolls back.
        return result;
    }
}
