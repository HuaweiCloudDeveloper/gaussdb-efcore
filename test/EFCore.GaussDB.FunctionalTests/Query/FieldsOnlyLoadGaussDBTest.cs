namespace Microsoft.EntityFrameworkCore.Query;

public class FieldsOnlyLoadGaussDBTest(FieldsOnlyLoadGaussDBTest.FieldsOnlyLoadGaussDBFixture fixture)
    : FieldsOnlyLoadTestBase<FieldsOnlyLoadGaussDBTest.FieldsOnlyLoadGaussDBFixture>(fixture)
{
    public class FieldsOnlyLoadGaussDBFixture : FieldsOnlyLoadFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
