namespace QueueSystem.Domain;

/// <summary>
/// Turns the zero-based sequence of a ticket within a round into its code.
/// The letter is the tens part (A–Z) and the digit is the units part (0–9),
/// so 0 → "A0", 9 → "A9", 10 → "B0" and 259 → "Z9".
/// </summary>
public static class TicketNumber
{
    /// <summary>26 letters × 10 digits gives sequences 0–259; 259 is "Z9".</summary>
    public const int MaxSequence = 259;

    /// <summary>Shown when no ticket has been issued in the current round.</summary>
    public const string Empty = "00";

    public static string Format(int sequence)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sequence);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sequence, MaxSequence);

        var letter = (char)('A' + sequence / 10);
        return $"{letter}{sequence % 10}";
    }
}
