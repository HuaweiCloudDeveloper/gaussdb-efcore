namespace Microsoft.EntityFrameworkCore.Update;

public class NonSharedModelUpdatesGaussDBTest(NonSharedFixture fixture) : NonSharedModelUpdatesTestBase(fixture)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
