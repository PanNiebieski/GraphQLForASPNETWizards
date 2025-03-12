using _05RestAPIDocumentViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Add this using directive
using Swashbuckle.AspNetCore.SwaggerGen; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=documentViews.db"));

builder.Services.AddScoped<IDocumentViewService, DocumentViewService>();

// Add endpoint mapping
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // This line requires the above using directives

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
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
        endpoints.MapGet("/{documentId}/{timeFrameS}", async (string documentId, string timeFrameS, IDocumentViewService service) =>
        {
            TimeFrame timeFrame = TimeFrame.Month;
            var result = await service.GetDocumentViewsOverTimeAsync(documentId, timeFrame);
            return Results.Ok(result);
        })
        .WithName("GetDocumentViewsOverTime")
        .WithDescription("Get document views statistics over a specified timeframe (Day, Week, or Month)");

        return endpoints;
    }
}