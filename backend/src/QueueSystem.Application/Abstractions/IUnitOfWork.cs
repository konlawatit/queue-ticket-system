namespace QueueSystem.Application.Abstractions;

/// <summary>
/// Port for running several repository calls as one database transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a transaction, runs <paramref name="work"/>, saves all changes and commits.
    /// If <paramref name="work"/> throws, nothing is saved and the transaction is rolled back.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken cancellationToken);
}
