namespace Microsoft.EntityFrameworkCore.Query;

public class TPCManyToManyQueryGaussDBTest : TPCManyToManyQueryRelationalTestBase<TPCManyToManyQueryGaussDBFixture>
{
    public TPCManyToManyQueryGaussDBTest(TPCManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
