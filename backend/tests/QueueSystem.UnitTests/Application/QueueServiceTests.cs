using QueueSystem.Application.Services;
using QueueSystem.Domain;
using QueueSystem.UnitTests.Application.Fakes;

namespace QueueSystem.UnitTests.Application;

public class QueueServiceTests
{
    private static readonly DateTimeOffset Created = new(2026, 11, 18, 1, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Now = new(2026, 11, 18, 7, 30, 0, TimeSpan.Zero);

    private readonly FakeQueueStore _store = new(QueueState.Create(Created));
    private readonly QueueService _service;

    public QueueServiceTests()
    {
        _service = new QueueService(_store, _store, new FixedTimeProvider(Now));
    }

    [Fact]
    public async Task IssueTicketAsync_LocksStateInsideTransactionAndSavesTicket()
    {
        var response = await _service.IssueTicketAsync();

        Assert.Equal("A0", response.Code);
        Assert.Equal(Now, response.IssuedAt);
        Assert.Equal(1, _store.LockedReads);
        Assert.Equal(0, _store.UnlockedReads);
        Assert.Equal(1, _store.Commits);
        var saved = Assert.Single(_store.SavedTickets);
        Assert.Equal("A0", saved.Code);
    }

    [Fact]
    public async Task IssueTicketAsync_ReturnsIdAssignedWhenSaved()
    {
        var first = await _service.IssueTicketAsync();
        var second = await _service.IssueTicketAsync();

        Assert.Equal(1, first.Id);
        Assert.Equal("A0", first.Code);
        Assert.Equal(2, second.Id);
        Assert.Equal("A1", second.Code);
    }

    [Fact]
    public async Task IssueTicketAsync_WhenFull_ThrowsAndSavesNothing()
    {
        for (var i = 0; i <= TicketNumber.MaxSequence; i++)
        {
            _store.State.IssueNext(Created);
        }

        await Assert.ThrowsAsync<QueueFullException>(() => _service.IssueTicketAsync());

        Assert.Equal(0, _store.Commits);
        Assert.Equal(1, _store.Rollbacks);
        Assert.Empty(_store.SavedTickets);
    }

    [Fact]
    public async Task ResetAsync_LocksStateInsideTransactionAndReturns00()
    {
        await _service.IssueTicketAsync();

        var response = await _service.ResetAsync();

        Assert.Equal("00", response.Code);
        Assert.Equal(2, _store.State.Round);
        Assert.Equal(Now, _store.State.UpdatedAt);
        Assert.Equal(2, _store.LockedReads);
        Assert.Equal(2, _store.Commits);
    }

    [Fact]
    public async Task GetCurrentAsync_ReadsWithoutLockAndReturnsLastCode()
    {
        await _service.IssueTicketAsync();
        await _service.IssueTicketAsync();

        var response = await _service.GetCurrentAsync();

        Assert.Equal("A1", response.Code);
        Assert.Equal(1, _store.UnlockedReads);
        Assert.Equal(2, _store.LockedReads); // only from the two issues
    }

    [Fact]
    public async Task GetCurrentAsync_BeforeAnyTicket_Returns00()
    {
        var response = await _service.GetCurrentAsync();

        Assert.Equal("00", response.Code);
    }

    [Fact]
    public async Task GetTicketAsync_ReturnsSavedTicket()
    {
        var issued = await _service.IssueTicketAsync();

        var found = await _service.GetTicketAsync(issued.Id);

        Assert.Equal(issued, found);
    }

    [Fact]
    public async Task GetTicketAsync_UnknownId_ReturnsNull()
    {
        var found = await _service.GetTicketAsync(999);

        Assert.Null(found);
    }
}
