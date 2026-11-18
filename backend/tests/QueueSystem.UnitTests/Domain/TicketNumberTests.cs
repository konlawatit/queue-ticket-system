using QueueSystem.Domain;

namespace QueueSystem.UnitTests.Domain;

public class TicketNumberTests
{
    [Theory]
    [InlineData(0, "A0")]
    [InlineData(1, "A1")]
    [InlineData(9, "A9")]
    [InlineData(10, "B0")]
    [InlineData(37, "D7")]
    [InlineData(250, "Z0")]
    [InlineData(259, "Z9")]
    public void Format_UsesLetterForTensAndDigitForUnits(int sequence, string expected)
    {
        Assert.Equal(expected, TicketNumber.Format(sequence));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(260)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Format_RejectsSequenceOutsideZeroTo259(int sequence)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TicketNumber.Format(sequence));
    }

    [Fact]
    public void Format_CoversA0ToZ9_UniqueAndInIssueOrder()
    {
        var codes = Enumerable.Range(0, TicketNumber.MaxSequence + 1)
            .Select(TicketNumber.Format)
            .ToList();

        Assert.Equal(260, codes.Count);
        Assert.Equal("A0", codes[0]);
        Assert.Equal("Z9", codes[^1]);
        Assert.All(codes, code => Assert.Matches("^[A-Z][0-9]$", code));
        Assert.Distinct(codes);
        Assert.Equal(codes.Order(StringComparer.Ordinal), codes);
    }
}
