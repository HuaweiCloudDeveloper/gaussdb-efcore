namespace Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson;

public class OwnedJsonTypeGaussDBFixture : OwnedJsonRelationalFixtureBase
{
    protected override string StoreName
        => "OwnedJsonTypeRelationshipsQueryTest";

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
