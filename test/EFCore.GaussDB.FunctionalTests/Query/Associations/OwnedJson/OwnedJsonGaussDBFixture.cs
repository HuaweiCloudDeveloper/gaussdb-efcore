namespace Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson;

public class OwnedJsonGaussDBFixture : OwnedJsonRelationalFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
