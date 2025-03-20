using _07GraphQLMutations;
using _07GraphQLMutations.ExtendObjectType;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<WatchlistRepository>();

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
    .AddHttpRequestInterceptor<HttpRequestInterceptor>()
    .AddTypeExtension<DocumentViewsExtendObjecType>()
    .AddType<DocumentType>()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>()
    .AddMutationConventions()
    .RegisterDbContextFactory<AppDbContext>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(HttpContext context,
        IRequestExecutor requestExecutor,
        OperationRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        // Retrieve the Authorization header
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            // Check if the header starts with "Basic "
            if (authHeader.ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                // Extract the Base64-encoded credentials
                var encodedCredentials = authHeader.ToString().Substring("Basic ".Length).Trim();

                // Decode the Base64 string
                var credentialBytes = Convert.FromBase64String(encodedCredentials);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length == 2)
                {
                    var username = credentials[0];
                    var password = credentials[1];

                    // Store the username as global state so that resolvers can access it
                    requestBuilder.AddGlobalState("UserName", username);
                    requestBuilder.AddGlobalState("Password", password);
                }
            }
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}

public class GlobalStateUserNameAttribute : GlobalStateAttribute
{
    public GlobalStateUserNameAttribute() : base("UserName")
    {
    }
}

public class GlobalStatePasswordAttribute : GlobalStateAttribute
{
    public GlobalStatePasswordAttribute() : base("Password")
    {
    }
}