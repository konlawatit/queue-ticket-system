using QueueSystem.Domain;

namespace QueueSystem.Application.Contracts;

/// <param name="Id">Used in the URL of the ticket page.</param>
/// <param name="Code">Ticket code such as "A5".</param>
/// <param name="IssuedAt">UTC; the frontend converts it to Asia/Bangkok for display.</param>
public sealed record TicketResponse(long Id, string Code, DateTimeOffset IssuedAt)
{
    internal static TicketResponse From(Ticket ticket) =>
        new(ticket.Id, ticket.Code, ticket.IssuedAt);
}
