namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.ObjectType;

public class ReviewType : ObjectType<Review>
{
    protected override void Configure(IObjectTypeDescriptor<Review> descriptor)
    {
        descriptor.Field(r => r.Id).Type<NonNullType<IntType>>();
        descriptor.Field(r => r.AuthorName).Type<NonNullType<StringType>>();
        descriptor.Field(r => r.Rating).Type<NonNullType<IntType>>();
        descriptor.Field(r => r.Comment).Type<StringType>();
        descriptor.Field(r => r.CreatedAt).Type<NonNullType<DateTimeType>>();
    }
}
