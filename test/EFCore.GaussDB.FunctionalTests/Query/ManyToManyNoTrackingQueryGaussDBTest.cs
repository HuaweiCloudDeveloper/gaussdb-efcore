namespace Microsoft.EntityFrameworkCore.Query;

internal class ManyToManyNoTrackingQueryGaussDBTest
    : ManyToManyNoTrackingQueryRelationalTestBase<ManyToManyQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public ManyToManyNoTrackingQueryGaussDBTest(ManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // TODO: #1232
    // protected override bool CanExecuteQueryString => true;
}
