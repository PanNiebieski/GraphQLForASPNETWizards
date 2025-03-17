using _03GraphQLDocumentsSortingOrdering;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddTypes(typeof(Query))
    .AddProjections()
    .AddSorting()
    .AddFiltering()
    .AddType<DocumentType>()
    .AddType<TagType>()
    .AddType<MetadataType>()
    .RegisterDbContextFactory<AppDbContext>();


var app = builder.Build();

app.MapGraphQL();

app.Run();


