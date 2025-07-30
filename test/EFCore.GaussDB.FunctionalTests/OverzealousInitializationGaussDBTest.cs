namespace Microsoft.EntityFrameworkCore;

public class OverzealousInitializationGaussDBTest(OverzealousInitializationGaussDBTest.OverzealousInitializationGaussDBFixture fixture)
    : OverzealousInitializationTestBase<OverzealousInitializationGaussDBTest.OverzealousInitializationGaussDBFixture>(fixture)
{
    public class OverzealousInitializationGaussDBFixture : OverzealousInitializationFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
