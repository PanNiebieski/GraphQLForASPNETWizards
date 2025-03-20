using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.Models;

namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.ObjectType;

public class ProductDetailsType : ObjectType<ProductDetails>
{
    protected override void Configure(IObjectTypeDescriptor<ProductDetails> descriptor)
    {
        descriptor.Field(d => d.Manufacturer).Type<StringType>();
        descriptor.Field(d => d.ManufactureDate).Type<DateTimeType>();
        descriptor.Field(d => d.CountryOfOrigin).Type<StringType>();
        descriptor.Field(d => d.MaterialInfo).Type<StringType>();
        descriptor.Field(d => d.Dimensions).Type<StringType>();
        descriptor.Field(d => d.TechnicalSpecifications).Type<StringType>();
    }
}