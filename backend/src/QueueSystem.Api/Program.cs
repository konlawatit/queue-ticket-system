using QueueSystem.Api.ErrorHandling;
using QueueSystem.Application;
using QueueSystem.Infrastructure;
using QueueSystem.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.Title = "Queue Ticket System API";
    document.Info.Description = "Issues queue tickets A0–Z9 without duplicates, even under concurrent requests.";
    return Task.CompletedTask;
}));

// Errors are returned as RFC 9457 ProblemDetails: 409 for a full queue (QueueExceptionHandler),
// 500 without a stack trace for anything unexpected, and empty 4xx responses get a body too.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<QueueExceptionHandler>();

// Liveness only: Playwright and Docker poll it to know when the API is accepting requests.
// Migrations run before the server starts listening, so a 200 also means the schema is ready.
builder.Services.AddHealthChecks();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await app.Services.MigrateDatabaseAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();              // /openapi/v1.json
    app.MapScalarApiReference();   // /scalar
}

app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();
