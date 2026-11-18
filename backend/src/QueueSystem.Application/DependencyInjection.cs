using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QueueSystem.Application.Services;

namespace QueueSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // TryAdd lets tests or the host register a fake clock before this runs.
        services.TryAddSingleton(TimeProvider.System);

        // Scoped: it depends on the repository and unit of work, which share one DbContext per request.
        services.AddScoped<IQueueService, QueueService>();

        return services;
    }
}
