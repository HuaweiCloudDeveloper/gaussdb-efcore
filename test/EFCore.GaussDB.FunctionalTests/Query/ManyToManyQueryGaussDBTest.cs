namespace Microsoft.EntityFrameworkCore.Query;

internal class ManyToManyQueryGaussDBTest : ManyToManyQueryRelationalTestBase<ManyToManyQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public ManyToManyQueryGaussDBTest(ManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
