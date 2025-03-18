using _05RestAPIDocumentViews;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=documentViews.db"));

builder.Services.AddScoped<IDocumentViewService, DocumentViewService>();
builder.Services.AddOpenApi();

// Add endpoint mapping
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseReDoc(c =>
    {
        c.SpecUrl("/openapi/v1.json");
    });
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGroup("/views")
    .MapDocumentViewEndpoints();

app.Run();


public static class DocumentViewEndpointExtensions
{
    public static IEndpointRouteBuilder MapDocumentViewEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/id/{documentId}/span/{timeFrameS:regex(^(week|month|threemonths)$)}",
            async (string documentId, string timeFrameS, IDocumentViewService service) =>
        {
            if (!Enum.TryParse<TimeFrame>(timeFrameS, true, out var timeFrame))
            {
                return Results.BadRequest
                ("Invalid time frame. Please use 'threemonths', 'week', or 'month'.");
            }
            var result = await service.GetDocumentViewsOverTimeAsync(documentId, timeFrame);
            return Results.Ok(result);
        });

        endpoints.MapGet("/span/{timeFrameS:regex(^(week|month|threemonths)$)}",
        async (string timeFrameS, IDocumentViewService service) =>
        {
            if (!Enum.TryParse<TimeFrame>(timeFrameS, true, out var timeFrame))
            {
                return Results.BadRequest
                ("Invalid time frame. Please use 'threemonths', 'week', or 'month'.");
            }
            var result = await service.GetDocumentsViewsOverTimeAsync(timeFrame);
            return Results.Ok(result);
        });

        return endpoints;
    }



}

