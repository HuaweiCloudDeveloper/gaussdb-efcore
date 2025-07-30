namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindAsNoTrackingQueryGaussDBTest : NorthwindAsNoTrackingQueryTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindAsNoTrackingQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
    }
}
