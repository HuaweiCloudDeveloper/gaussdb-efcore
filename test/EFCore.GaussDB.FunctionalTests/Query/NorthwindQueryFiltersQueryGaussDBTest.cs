namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindQueryFiltersQueryGaussDBTest : NorthwindQueryFiltersQueryTestBase<
    NorthwindQueryGaussDBFixture<NorthwindQueryFiltersCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindQueryFiltersQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NorthwindQueryFiltersCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
