using Microsoft.AspNetCore.Http.HttpResults;
using QueueSystem.Application.Contracts;
using QueueSystem.Application.Services;

namespace QueueSystem.Api.Endpoints;

public static class QueueEndpoints
{
    private const string GetTicketRoute = "GetTicket";

    public static IEndpointRouteBuilder MapQueueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/queue").WithTags("Queue");

        group.MapPost("/tickets", IssueTicketAsync)
            .WithName("IssueTicket")
            .WithSummary("Issue the next ticket (IT 05-1)")
            .WithDescription("Safe to call concurrently: every caller gets a different code. Returns 409 once Z9 has been issued.")
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/tickets/{id:long}", GetTicketAsync)
            .WithName(GetTicketRoute)
            .WithSummary("Get an issued ticket (IT 05-2)");

        group.MapGet("/current", GetCurrentAsync)
            .WithName("GetCurrent")
            .WithSummary("Get the last ticket code of the current round, or \"00\" (IT 05-3)");

        group.MapPost("/reset", ResetAsync)
            .WithName("ResetQueue")
            .WithSummary("Start a new round; the current code becomes \"00\" (IT 05-3)");

        return app;
    }

    private static async Task<CreatedAtRoute<TicketResponse>> IssueTicketAsync(
        IQueueService queue,
        CancellationToken cancellationToken)
    {
        var ticket = await queue.IssueTicketAsync(cancellationToken);
        return TypedResults.CreatedAtRoute(ticket, GetTicketRoute, new { id = ticket.Id });
    }

    private static async Task<Results<Ok<TicketResponse>, NotFound>> GetTicketAsync(
        long id,
        IQueueService queue,
        CancellationToken cancellationToken)
    {
        var ticket = await queue.GetTicketAsync(id, cancellationToken);
        return ticket is null ? TypedResults.NotFound() : TypedResults.Ok(ticket);
    }

    private static async Task<Ok<CurrentQueueResponse>> GetCurrentAsync(
        IQueueService queue,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await queue.GetCurrentAsync(cancellationToken));

    private static async Task<Ok<CurrentQueueResponse>> ResetAsync(
        IQueueService queue,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await queue.ResetAsync(cancellationToken));
}
