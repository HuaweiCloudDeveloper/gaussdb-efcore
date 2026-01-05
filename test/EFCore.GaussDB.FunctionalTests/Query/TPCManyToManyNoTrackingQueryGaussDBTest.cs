namespace Microsoft.EntityFrameworkCore.Query;

public class TPCManyToManyNoTrackingQueryGaussDBTest : TPCManyToManyNoTrackingQueryRelationalTestBase<TPCManyToManyQueryGaussDBFixture>
{
    public TPCManyToManyNoTrackingQueryGaussDBTest(TPCManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
