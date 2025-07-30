namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindNavigationsQueryGaussDBTest : NorthwindNavigationsQueryRelationalTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindNavigationsQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
