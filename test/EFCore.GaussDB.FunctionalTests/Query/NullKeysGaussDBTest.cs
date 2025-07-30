namespace Microsoft.EntityFrameworkCore.Query;

public class NullKeysGaussDBTest(NullKeysGaussDBTest.NullKeysGaussDBFixture fixture)
    : NullKeysTestBase<NullKeysGaussDBTest.NullKeysGaussDBFixture>(fixture)
{
    public class NullKeysGaussDBFixture : NullKeysFixtureBase
    {
        protected override string StoreName { get; } = "StringsContext";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
