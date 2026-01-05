namespace Microsoft.EntityFrameworkCore.ModelBuilding;

public class GaussDBModelBuilderTestBase : RelationalModelBuilderTest
{
    public abstract class GaussDBNonRelationship(GaussDBModelBuilderFixture fixture)
        : RelationalNonRelationshipTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBComplexType(GaussDBModelBuilderFixture fixture)
        : RelationalComplexTypeTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBComplexCollection(GaussDBModelBuilderFixture fixture)
        : RelationalComplexCollectionTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBInheritance(GaussDBModelBuilderFixture fixture)
        : RelationalInheritanceTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBOneToMany(GaussDBModelBuilderFixture fixture)
        : RelationalOneToManyTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBManyToOne(GaussDBModelBuilderFixture fixture)
        : RelationalManyToOneTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBOneToOne(GaussDBModelBuilderFixture fixture)
        : RelationalOneToOneTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBManyToMany(GaussDBModelBuilderFixture fixture)
        : RelationalManyToManyTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public abstract class GaussDBOwnedTypes(GaussDBModelBuilderFixture fixture)
        : RelationalOwnedTypesTestBase(fixture), IClassFixture<GaussDBModelBuilderFixture>;

    public class GaussDBModelBuilderFixture : RelationalModelBuilderFixture
    {
        public override TestHelpers TestHelpers
            => GaussDBTestHelpers.Instance;
    }
}
