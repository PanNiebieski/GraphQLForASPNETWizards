using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.Models;

namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes;

// Query class that returns the basic product info immediately.
public class Query
{
    // The product field takes an id argument.
    public Product GetProduct(int id)
    {
        return new Product
        {
            Id = id,
            Name = "SmartPhone X",
            Price = 999.99m,
            Description = "Latest smartphone with cutting-edge features"
            // Details and Reviews are intentionally not populated here.
        };
    }
}