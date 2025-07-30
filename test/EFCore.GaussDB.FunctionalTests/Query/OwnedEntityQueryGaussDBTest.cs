namespace Microsoft.EntityFrameworkCore.Query;

public class OwnedEntityQueryGaussDBTest(NonSharedFixture fixture) : OwnedEntityQueryRelationalTestBase(fixture)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
