using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.Models;

namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.ObjectType;

// GraphQL Schema Types
public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Field(p => p.Id)
            .Type<NonNullType<IntType>>();
        descriptor.Field(p => p.Name)
            .Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Price)
            .Type<NonNullType<DecimalType>>();
        descriptor.Field(p => p.Description)
            .Type<StringType>();

        // Use separate resolvers for Details and Reviews so that
        // they can be deferred or streamed.
        descriptor.Field("details")
            .ResolveWith<ProductResolvers>(r => r.GetDetails(default))
            .Type<ProductDetailsType>();
        descriptor.Field("reviews")
            .ResolveWith<ProductResolvers>(r => r.GetReviews(default))
            .Type<ListType<ReviewType>>();
    }
}