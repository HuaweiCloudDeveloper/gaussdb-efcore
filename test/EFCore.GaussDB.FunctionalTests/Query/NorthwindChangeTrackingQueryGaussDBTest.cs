using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindChangeTrackingQueryGaussDBTest : NorthwindChangeTrackingQueryTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindChangeTrackingQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override NorthwindContext CreateNoTrackingContext()
        => new NorthwindGaussDBContext(
            new DbContextOptionsBuilder(Fixture.CreateOptions())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
}
