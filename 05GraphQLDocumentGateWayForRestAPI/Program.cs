using _05GraphQLDocumentGateWayForRestAPI;
using _05GraphQLDocumentGateWayForRestAPI.ExtendObjectType;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("documentViews", client =>
{
    client.BaseAddress = new Uri("https://localhost:2222");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .AddSorting()
    .AddFiltering()
    .AddTypeExtension<DocumentViewsExtendObjecType>()
    .AddType<DocumentType>()
    .AddTypes(typeof(Query))
    .RegisterDbContextFactory<AppDbContext>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

