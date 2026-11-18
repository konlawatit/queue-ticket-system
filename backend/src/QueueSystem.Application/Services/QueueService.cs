using QueueSystem.Application.Abstractions;
using QueueSystem.Application.Contracts;

namespace QueueSystem.Application.Services;

/// <summary>
/// Coordinates the use cases. The numbering rules themselves live in
/// <see cref="Domain.QueueState"/>; this class only decides how the state is
/// loaded (with or without a lock) and when changes are committed.
/// </summary>
internal sealed class QueueService(
    IQueueRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IQueueService
{
    public async Task<TicketResponse> IssueTicketAsync(CancellationToken cancellationToken = default)
    {
        var ticket = await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var state = await repository.GetStateForUpdateAsync(token);
            var issued = state.IssueNext(timeProvider.GetUtcNow());
            repository.AddTicket(issued);
            return issued;
        }, cancellationToken);

        // Map after the commit: the database assigns Ticket.Id when the changes are saved.
        return TicketResponse.From(ticket);
    }

    public async Task<TicketResponse?> GetTicketAsync(long id, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.FindTicketAsync(id, cancellationToken);
        return ticket is null ? null : TicketResponse.From(ticket);
    }

    public async Task<CurrentQueueResponse> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var state = await repository.GetStateAsync(cancellationToken);
        return new CurrentQueueResponse(state.CurrentCode);
    }

    public async Task<CurrentQueueResponse> ResetAsync(CancellationToken cancellationToken = default)
    {
        // Takes the same lock as IssueTicketAsync, so a reset never interleaves with issuing.
        var state = await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var locked = await repository.GetStateForUpdateAsync(token);
            locked.Reset(timeProvider.GetUtcNow());
            return locked;
        }, cancellationToken);

        return new CurrentQueueResponse(state.CurrentCode);
    }
}
