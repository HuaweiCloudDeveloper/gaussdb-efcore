namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindAsTrackingQueryGaussDBTest : NorthwindAsTrackingQueryTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindAsTrackingQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
    }
}
