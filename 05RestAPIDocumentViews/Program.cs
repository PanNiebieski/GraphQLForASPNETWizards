using _05RestAPIDocumentViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=documentViews.db"));

builder.Services.AddScoped<IDocumentViewService, DocumentViewService>();
builder.Services.AddOpenApi();

// Add endpoint mapping
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/openapi/v1.json", "Document Views API");
    //});

    app.UseReDoc(c =>
    {
        c.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Define endpoints using new .NET 9 features for minimal APIs
app.MapGroup("/views")
    .MapDocumentViewEndpoints();

app.Run();


public static class DocumentViewEndpointExtensions
{
    public static IEndpointRouteBuilder MapDocumentViewEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{documentId}/{timeFrameS:regex(^(day|week|month)$)}", async (string documentId, string timeFrameS, IDocumentViewService service) =>
        {
            if (!Enum.TryParse<TimeFrame>(timeFrameS, true, out var timeFrame))
            {
                return Results.BadRequest("Invalid time frame. Please use 'day', 'week', or 'month'.");
            }
            var result = await service.GetDocumentViewsOverTimeAsync(documentId, timeFrame);
            return Results.Ok(result);
        })
        .WithName("GetDocumentViewsOverTime")
        .WithDescription("Get document views statistics over a specified timeframe (Day, Week, or Month)")
        .WithOpenApi(operation =>
        {
            operation.Parameters[1].Schema = new OpenApiSchema();
            operation.Parameters[1].Schema.Enum = new List<IOpenApiAny>
            {
                new OpenApiString("day"),
                new OpenApiString("week"),
                new OpenApiString("month")
            };
            operation.Parameters[1].Description = "Timeframe for views statistics. Must be one of: day, week, month";
            return operation;
        });
        return endpoints;
    }
}