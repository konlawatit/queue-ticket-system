using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueSystem.Domain;
using static System.FormattableString;

namespace QueueSystem.Infrastructure.Persistence.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets", table =>
            table.HasCheckConstraint(
                "CK_Tickets_Sequence",
                Invariant($"[Sequence] BETWEEN 0 AND {TicketNumber.MaxSequence}")));

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code)
            .HasMaxLength(2)
            .IsUnicode(false);

        // Last line of defence: even if the lock were bypassed, the database
        // refuses a second ticket with the same number in the same round.
        builder.HasIndex(t => new { t.Round, t.Sequence })
            .IsUnique()
            .HasDatabaseName("UX_Tickets_Round_Sequence");
    }
}
