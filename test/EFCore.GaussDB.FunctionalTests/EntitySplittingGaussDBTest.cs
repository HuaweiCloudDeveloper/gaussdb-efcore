namespace Microsoft.EntityFrameworkCore;

public class EntitySplittingGaussDBTest(NonSharedFixture fixture, ITestOutputHelper testOutputHelper)
    : EntitySplittingTestBase(fixture, testOutputHelper)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
