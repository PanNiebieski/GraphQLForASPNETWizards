using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes;
using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.ObjectType;

var builder = WebApplication.CreateBuilder(args);

// Register GraphQL services
builder.Services.AddGraphQLServer()
    .ModifyOptions(o =>
    {
        o.EnableDefer = true;
        o.EnableStream = true;
    }).AddQueryType<Query>()
            .AddType<ProductType>()
            .AddType<ProductDetailsType>()
            .AddType<ReviewType>();

var app = builder.Build();

app.MapGraphQL();
app.MapGet("/", () =>
{ return Results.Redirect($"/graphql", permanent: true); });
app.Run();