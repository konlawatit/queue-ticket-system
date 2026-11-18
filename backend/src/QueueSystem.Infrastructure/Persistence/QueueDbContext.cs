using Microsoft.EntityFrameworkCore;
using QueueSystem.Domain;

namespace QueueSystem.Infrastructure.Persistence;

internal sealed class QueueDbContext(DbContextOptions<QueueDbContext> options) : DbContext(options)
{
    public DbSet<QueueState> QueueStates => Set<QueueState>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QueueDbContext).Assembly);
}
