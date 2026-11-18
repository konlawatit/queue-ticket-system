namespace QueueSystem.UnitTests.Application.Fakes;

/// <summary>A clock that always returns the same instant.</summary>
internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
