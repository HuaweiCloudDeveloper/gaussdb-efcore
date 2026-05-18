namespace Microsoft.EntityFrameworkCore.Update;

public class NonSharedModelUpdatesGaussDBTest : NonSharedModelUpdatesTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
