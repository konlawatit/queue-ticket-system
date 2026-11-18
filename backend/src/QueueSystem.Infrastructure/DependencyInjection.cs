using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueueSystem.Application.Abstractions;
using QueueSystem.Infrastructure.Persistence;

namespace QueueSystem.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "QueueDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is missing. " +
                $"For local development run: dotnet user-secrets set \"ConnectionStrings:{ConnectionStringName}\" \"<connection string>\" --project src/QueueSystem.Api. " +
                $"Elsewhere set the ConnectionStrings__{ConnectionStringName} environment variable.");

        services.AddDbContext<QueueDbContext>(options => options.UseSqlServer(connectionString));

        // One DbContext per request is shared by the repository and the unit of work,
        // so the tickets added by the repository are saved by the unit of work.
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
