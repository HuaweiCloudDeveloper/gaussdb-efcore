namespace Microsoft.EntityFrameworkCore;

public class CompositeKeyEndToEndGaussDBTest(CompositeKeyEndToEndGaussDBTest.CompositeKeyEndToEndGaussDBFixture fixture)
    : CompositeKeyEndToEndTestBase<CompositeKeyEndToEndGaussDBTest.CompositeKeyEndToEndGaussDBFixture>(fixture)
{
    public class CompositeKeyEndToEndGaussDBFixture : CompositeKeyEndToEndFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
