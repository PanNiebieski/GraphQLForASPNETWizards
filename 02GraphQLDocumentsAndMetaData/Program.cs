using _02GraphQLDocumentsAndMetaData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddTypes(typeof(Query))
    .RegisterDbContextFactory<AppDbContext>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
