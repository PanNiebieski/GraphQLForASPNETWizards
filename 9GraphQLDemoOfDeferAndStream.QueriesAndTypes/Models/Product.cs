namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.Models;

// Domain models
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    // Details and Reviews are no longer populated in the query resolver
    // but resolved on-demand via field resolvers.
    public ProductDetails Details { get; set; }
    public List<Review> Reviews { get; set; }
}
