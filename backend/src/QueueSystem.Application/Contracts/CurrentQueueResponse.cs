namespace QueueSystem.Application.Contracts;

/// <param name="Code">Last ticket issued in the current round, or "00" if there is none.</param>
public sealed record CurrentQueueResponse(string Code);
