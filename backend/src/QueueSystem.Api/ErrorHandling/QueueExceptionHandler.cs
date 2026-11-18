using Microsoft.AspNetCore.Diagnostics;
using QueueSystem.Domain;

namespace QueueSystem.Api.ErrorHandling;

/// <summary>
/// Turns the expected "queue is full" case into a 409 ProblemDetails response.
/// Any other exception is left to the default handler, which returns a 500
/// ProblemDetails without the stack trace.
/// </summary>
internal sealed class QueueExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not QueueFullException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Queue is full",
                Detail = "All tickets A0–Z9 have been issued. Reset the queue to continue.",
            },
        });
    }
}
