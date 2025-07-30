namespace Microsoft.EntityFrameworkCore.Query;

public class AdHocQueryFiltersQueryGaussDBTest(NonSharedFixture fixture)
    : AdHocQueryFiltersQueryRelationalTestBase(fixture)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
