namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindCompiledQueryGaussDBTest : NorthwindCompiledQueryTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindCompiledQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
