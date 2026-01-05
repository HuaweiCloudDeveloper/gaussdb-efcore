namespace Microsoft.EntityFrameworkCore.Query;

public class FiltersInheritanceQueryGaussDBTest : FiltersInheritanceQueryTestBase<TPHFiltersInheritanceQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public FiltersInheritanceQueryGaussDBTest(TPHFiltersInheritanceQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }
}
