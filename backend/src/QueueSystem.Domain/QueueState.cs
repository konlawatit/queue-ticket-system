namespace QueueSystem.Domain;

/// <summary>
/// The single counter row (Id = 1) that decides which ticket comes next.
/// Every numbering rule lives here so it can be tested without a database;
/// the infrastructure layer only has to load this row under a lock and save it.
/// </summary>
public sealed class QueueState
{
    public const int SingletonId = 1;

    /// <summary>Value of <see cref="LastSequence"/> before the first ticket of a round.</summary>
    public const int NoTicketIssued = -1;

    // Used by EF Core when loading the row.
    private QueueState()
    {
    }

    public int Id { get; private set; }

    /// <summary>Starts at 1 and goes up by one on every reset, so old tickets are kept.</summary>
    public int Round { get; private set; }

    /// <summary>Sequence of the last ticket issued in this round, or <see cref="NoTicketIssued"/>.</summary>
    public int LastSequence { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsFull => LastSequence >= TicketNumber.MaxSequence;

    /// <summary>Code of the last ticket issued in this round, or "00" if there is none.</summary>
    public string CurrentCode =>
        LastSequence == NoTicketIssued ? TicketNumber.Empty : TicketNumber.Format(LastSequence);

    public static QueueState Create(DateTimeOffset now) => new()
    {
        Id = SingletonId,
        Round = 1,
        LastSequence = NoTicketIssued,
        UpdatedAt = now,
    };

    /// <exception cref="QueueFullException">Z9 has already been issued in this round.</exception>
    public Ticket IssueNext(DateTimeOffset now)
    {
        if (IsFull)
        {
            throw new QueueFullException();
        }

        LastSequence++;
        UpdatedAt = now;
        return Ticket.Issue(Round, LastSequence, now);
    }

    /// <summary>Starts a new round: the screen shows "00" and the next ticket is A0.</summary>
    public void Reset(DateTimeOffset now)
    {
        Round++;
        LastSequence = NoTicketIssued;
        UpdatedAt = now;
    }
}
