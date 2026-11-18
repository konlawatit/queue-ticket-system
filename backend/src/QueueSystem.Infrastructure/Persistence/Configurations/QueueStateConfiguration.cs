using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueSystem.Domain;
using static System.FormattableString;

namespace QueueSystem.Infrastructure.Persistence.Configurations;

internal sealed class QueueStateConfiguration : IEntityTypeConfiguration<QueueState>
{
    // A fixed value keeps the seed data identical in every migration.
    private static readonly DateTimeOffset SeededAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<QueueState> builder)
    {
        // Invariant(): a culture with a different minus sign must not change the SQL.
        builder.ToTable("QueueStates", table =>
        {
            table.HasCheckConstraint(
                "CK_QueueStates_Singleton",
                Invariant($"[Id] = {QueueState.SingletonId}"));
            table.HasCheckConstraint(
                "CK_QueueStates_LastSequence",
                Invariant($"[LastSequence] BETWEEN {QueueState.NoTicketIssued} AND {TicketNumber.MaxSequence}"));
        });

        builder.HasKey(s => s.Id);

        // The single row always has Id = 1, so it must not be an IDENTITY column.
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Ignore(s => s.IsFull);
        builder.Ignore(s => s.CurrentCode);

        builder.HasData(QueueState.Create(SeededAt));
    }
}
