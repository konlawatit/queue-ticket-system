using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace QueueSystem.Infrastructure.Persistence;

public static class DatabaseMigrator
{
    /// <summary>
    /// Applies pending EF Core migrations (creates the database if needed).
    /// Used when Database:MigrateOnStartup is true: local development, E2E tests and Docker.
    /// EF Core holds a database lock while migrating, so two instances starting at
    /// the same time do not apply the same migration twice.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }
}
