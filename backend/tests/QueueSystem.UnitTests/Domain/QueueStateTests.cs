using QueueSystem.Domain;

namespace QueueSystem.UnitTests.Domain;

public class QueueStateTests
{
    private static readonly DateTimeOffset T0 = new(2026, 11, 18, 3, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_StartsRoundOneWithNoTicket()
    {
        var state = QueueState.Create(T0);

        Assert.Equal(QueueState.SingletonId, state.Id);
        Assert.Equal(1, state.Round);
        Assert.Equal(-1, state.LastSequence);
        Assert.Equal("00", state.CurrentCode);
        Assert.False(state.IsFull);
        Assert.Equal(T0, state.UpdatedAt);
    }

    [Fact]
    public void IssueNext_FirstTicketIsA0()
    {
        var state = QueueState.Create(T0);
        var issuedAt = T0.AddMinutes(5);

        var ticket = state.IssueNext(issuedAt);

        Assert.Equal("A0", ticket.Code);
        Assert.Equal(0, ticket.Sequence);
        Assert.Equal(1, ticket.Round);
        Assert.Equal(issuedAt, ticket.IssuedAt);
        Assert.Equal("A0", state.CurrentCode);
        Assert.Equal(issuedAt, state.UpdatedAt);
    }

    [Fact]
    public void IssueNext_AfterA9_ReturnsB0()
    {
        var state = QueueState.Create(T0);
        IssueMany(state, 10); // A0..A9
        Assert.Equal("A9", state.CurrentCode);

        var ticket = state.IssueNext(T0);

        Assert.Equal("B0", ticket.Code);
    }

    [Fact]
    public void IssueNext_IssuesEveryCodeInOrderUpToZ9()
    {
        var state = QueueState.Create(T0);

        var codes = IssueMany(state, 260).Select(t => t.Code);

        Assert.Equal(Enumerable.Range(0, 260).Select(TicketNumber.Format), codes);
        Assert.Equal("Z9", state.CurrentCode);
        Assert.True(state.IsFull);
    }

    [Fact]
    public void IssueNext_AfterZ9_ThrowsAndLeavesStateUnchanged()
    {
        var state = QueueState.Create(T0);
        IssueMany(state, 260);
        var updatedAt = state.UpdatedAt;

        Assert.Throws<QueueFullException>(() => state.IssueNext(T0.AddHours(1)));

        Assert.Equal(TicketNumber.MaxSequence, state.LastSequence);
        Assert.Equal("Z9", state.CurrentCode);
        Assert.Equal(updatedAt, state.UpdatedAt);
    }

    [Fact]
    public void Reset_ShowsEmptyCodeAndStartsNextRound()
    {
        var state = QueueState.Create(T0);
        IssueMany(state, 6); // A0..A5
        var resetAt = T0.AddHours(1);

        state.Reset(resetAt);

        Assert.Equal("00", state.CurrentCode);
        Assert.Equal(2, state.Round);
        Assert.Equal(-1, state.LastSequence);
        Assert.False(state.IsFull);
        Assert.Equal(resetAt, state.UpdatedAt);
    }

    [Fact]
    public void IssueNext_AfterReset_StartsFromA0InNewRound()
    {
        var state = QueueState.Create(T0);
        IssueMany(state, 6);
        state.Reset(T0);

        var ticket = state.IssueNext(T0);

        Assert.Equal("A0", ticket.Code);
        Assert.Equal(2, ticket.Round);
    }

    [Fact]
    public void Reset_WhenFull_AllowsIssuingAgain()
    {
        var state = QueueState.Create(T0);
        IssueMany(state, 260);

        state.Reset(T0);
        var ticket = state.IssueNext(T0);

        Assert.Equal("A0", ticket.Code);
        Assert.False(state.IsFull);
    }

    private static List<Ticket> IssueMany(QueueState state, int count)
    {
        var tickets = new List<Ticket>(count);
        for (var i = 0; i < count; i++)
        {
            tickets.Add(state.IssueNext(T0));
        }

        return tickets;
    }
}
