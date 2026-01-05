namespace Microsoft.EntityFrameworkCore.Query;

public class AdHocManyToManyQueryGaussDBTest(NonSharedFixture fixture) : AdHocManyToManyQueryRelationalTestBase(fixture)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
