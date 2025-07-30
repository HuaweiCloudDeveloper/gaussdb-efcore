namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindSplitIncludeNoTrackingQueryGaussDBTest : NorthwindSplitIncludeNoTrackingQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindSplitIncludeNoTrackingQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        // TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
    }
}
