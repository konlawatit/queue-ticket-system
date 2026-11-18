using System.Reflection;
using QueueSystem.Application.Abstractions;
using QueueSystem.Domain;

namespace QueueSystem.UnitTests.Application.Fakes;

/// <summary>
/// In-memory stand-in for the repository and unit of work. Like the real
/// DbContext-based implementation, one object plays both roles so that tickets
/// added during a transaction are saved when it commits.
/// It also records how the state was read, so tests can prove that the
/// service locks the counter row and only does so inside a transaction.
/// </summary>
internal sealed class FakeQueueStore(QueueState state) : IQueueRepository, IUnitOfWork
{
    private static readonly PropertyInfo TicketIdProperty = typeof(Ticket).GetProperty(nameof(Ticket.Id))!;

    private readonly List<Ticket> _pending = [];
    private readonly List<Ticket> _saved = [];
    private long _nextId = 1;
    private bool _inTransaction;

    public QueueState State { get; } = state;

    public IReadOnlyList<Ticket> SavedTickets => _saved;

    public int LockedReads { get; private set; }

    public int UnlockedReads { get; private set; }

    public int Commits { get; private set; }

    public int Rollbacks { get; private set; }

    public Task<QueueState> GetStateForUpdateAsync(CancellationToken cancellationToken)
    {
        if (!_inTransaction)
        {
            throw new InvalidOperationException(
                "Locked read outside a transaction: the lock would be released straight away.");
        }

        LockedReads++;
        return Task.FromResult(State);
    }

    public Task<QueueState> GetStateAsync(CancellationToken cancellationToken)
    {
        UnlockedReads++;
        return Task.FromResult(State);
    }

    public Task<Ticket?> FindTicketAsync(long id, CancellationToken cancellationToken) =>
        Task.FromResult(_saved.SingleOrDefault(t => t.Id == id));

    public void AddTicket(Ticket ticket) => _pending.Add(ticket);

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken cancellationToken)
    {
        if (_inTransaction)
        {
            throw new InvalidOperationException("Nested transactions are not supported.");
        }

        _inTransaction = true;
        try
        {
            var result = await work(cancellationToken);

            // "SaveChanges": the database assigns identity values. EF Core sets the
            // private setter the same way when it reads the generated key back.
            foreach (var ticket in _pending)
            {
                TicketIdProperty.SetValue(ticket, _nextId++);
                _saved.Add(ticket);
            }

            _pending.Clear();
            Commits++;
            return result;
        }
        catch
        {
            _pending.Clear();
            Rollbacks++;
            throw;
        }
        finally
        {
            _inTransaction = false;
        }
    }
}
