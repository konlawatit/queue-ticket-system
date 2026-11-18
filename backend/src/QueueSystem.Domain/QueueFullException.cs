namespace QueueSystem.Domain;

/// <summary>
/// Thrown when the last code of the round (Z9) has already been issued.
/// The queue must be reset before another ticket can be issued.
/// </summary>
public sealed class QueueFullException()
    : InvalidOperationException("The queue is full: ticket Z9 has already been issued in this round. Reset the queue to start again.");
