namespace QueueSystem.Domain;

/// <summary>
/// A ticket that has been issued. Tickets are never deleted, so they also
/// serve as the history of every round.
/// </summary>
public sealed class Ticket
{
    // Used by EF Core when loading a ticket.
    private Ticket()
    {
    }

    /// <summary>Assigned by the database; used in the URL of the ticket page.</summary>
    public long Id { get; private set; }

    public int Round { get; private set; }

    public int Sequence { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public DateTimeOffset IssuedAt { get; private set; }

    // Only QueueState may create tickets, so a ticket can never skip the numbering rules.
    internal static Ticket Issue(int round, int sequence, DateTimeOffset issuedAt) => new()
    {
        Round = round,
        Sequence = sequence,
        Code = TicketNumber.Format(sequence),
        IssuedAt = issuedAt,
    };
}
