using Microsoft.AspNetCore.Mvc;
using QueueSystem.Application.Contracts;
using QueueSystem.Application.Services;

namespace QueueSystem.Api.Controllers;

/// <summary>
/// The four use cases of the kiosk. The controller only translates HTTP to service
/// calls; the numbering rules and the locking live in the Domain and Infrastructure layers.
/// A full queue (QueueFullException) becomes a 409 in ErrorHandling/QueueExceptionHandler.
/// </summary>
[ApiController]
[Route("api/queue")]
public sealed class QueueController(IQueueService queue) : ControllerBase
{
    // Action names have no "Async" suffix on purpose: MVC strips that suffix from
    // action names, so CreatedAtAction(nameof(GetTicketAsync)) would not find the route.
    //
    // Content types are declared per response, not with [Produces] on the class:
    // [Produces("application/json")] would also force error responses to
    // application/json instead of application/problem+json.

    [HttpPost("tickets")]
    [EndpointSummary("Issue the next ticket (IT 05-1)")]
    [EndpointDescription("Safe to call concurrently: every caller gets a different code. Returns 409 once Z9 has been issued.")]
    [ProducesResponseType<TicketResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<ActionResult<TicketResponse>> IssueTicket(CancellationToken cancellationToken)
    {
        var ticket = await queue.IssueTicketAsync(cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpGet("tickets/{id:long}")]
    [EndpointSummary("Get an issued ticket (IT 05-2)")]
    [ProducesResponseType<TicketResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<TicketResponse>> GetTicket(long id, CancellationToken cancellationToken)
    {
        var ticket = await queue.GetTicketAsync(id, cancellationToken);
        return ticket is null ? NotFound() : ticket;
    }

    [HttpGet("current")]
    [EndpointSummary("Get the last ticket code of the current round, or \"00\" (IT 05-3)")]
    [ProducesResponseType<CurrentQueueResponse>(StatusCodes.Status200OK, "application/json")]
    public Task<CurrentQueueResponse> GetCurrent(CancellationToken cancellationToken) =>
        queue.GetCurrentAsync(cancellationToken);

    [HttpPost("reset")]
    [EndpointSummary("Start a new round; the current code becomes \"00\" (IT 05-3)")]
    [ProducesResponseType<CurrentQueueResponse>(StatusCodes.Status200OK, "application/json")]
    public Task<CurrentQueueResponse> Reset(CancellationToken cancellationToken) =>
        queue.ResetAsync(cancellationToken);
}
