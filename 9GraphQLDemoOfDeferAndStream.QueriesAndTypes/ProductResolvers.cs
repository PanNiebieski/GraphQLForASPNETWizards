using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.Models;

namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes;

// Field resolvers for deferred and streamed fields.
public class ProductResolvers
{
    // Simulate slow database access for details (e.g. 14 seconds delay)
    public async Task<ProductDetails> GetDetails([Parent] Product product)
    {
        await Task.Delay(14000);
        return new ProductDetails
        {
            Manufacturer = "TechCorp",
            ManufactureDate = new DateTime(2024, 3, 15),
            CountryOfOrigin = "Japan",
            MaterialInfo = "Aluminum and glass",
            Dimensions = "6.1 x 2.8 x 0.3 inches",
            TechnicalSpecifications = "CPU: 3.2 GHz, RAM: 8GB, Storage: 256GB"
        };
    }

    // Simulate streaming reviews with a 2s delay for each review.
    public async IAsyncEnumerable<Review> GetReviews([Parent] Product product)
    {
        var reviews = new List<Review>
            {
                new Review { Id = 1, AuthorName = "Alice", Rating = 5, Comment = "Excellent product!", CreatedAt = DateTime.Now.AddDays(-5) },
                new Review { Id = 2, AuthorName = "Bob", Rating = 4, Comment = "Good value for money", CreatedAt = DateTime.Now.AddDays(-10) },
                new Review { Id = 3, AuthorName = "Charlie", Rating = 3, Comment = "Average performance", CreatedAt = DateTime.Now.AddDays(-15) },
                new Review { Id = 4, AuthorName = "David", Rating = 5, Comment = "Exceeded expectations", CreatedAt = DateTime.Now.AddDays(-20) },
                new Review { Id = 5, AuthorName = "Eve", Rating = 2, Comment = "Not worth the price", CreatedAt = DateTime.Now.AddDays(-25) }
            };

        foreach (var review in reviews)
        {
            await Task.Delay(2000);
            yield return review;
        }
    }
}