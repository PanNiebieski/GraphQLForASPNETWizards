var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();
app.MapGraphQL();

app.MapGet("/", () =>
{ return Results.Redirect($"/graphql", permanent: true); });

app.Run();

public class Query
{
    public string Hello(string name = "World",
        int a = 0, int b = 0)
    {
        int hour = DateTime.UtcNow.Hour;
        string timeMessage = hour == 0 ? "midnight" :
               hour == 12 ? "noon" :
               $"{(hour > 12 ? hour - 12 : hour)}" +
               $"{(hour >= 12 ? "PM" : "AM")}";

        string calMessage = a == 0 && b == 0 ? "" :
            $"The sum of {a} and {b} is {a + b} and ";

        return $"Hello, {name}! {calMessage} " +
            $"It's {timeMessage}.";
    }
}