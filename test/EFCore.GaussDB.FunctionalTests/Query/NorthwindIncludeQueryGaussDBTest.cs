namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindIncludeQueryGaussDBTest : NorthwindIncludeQueryRelationalTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindIncludeQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
