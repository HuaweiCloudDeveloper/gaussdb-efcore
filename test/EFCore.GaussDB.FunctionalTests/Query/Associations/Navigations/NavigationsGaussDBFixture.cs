namespace Microsoft.EntityFrameworkCore.Query.Associations.Navigations;

public class NavigationsGaussDBFixture : NavigationsRelationalFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
