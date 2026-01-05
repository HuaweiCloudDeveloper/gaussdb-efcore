namespace Microsoft.EntityFrameworkCore.Query;

public class TPTManyToManyNoTrackingQueryGaussDBTest : TPTManyToManyNoTrackingQueryRelationalTestBase<TPTManyToManyQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public TPTManyToManyNoTrackingQueryGaussDBTest(TPTManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // TODO: #1232
    // protected override bool CanExecuteQueryString => true;
}
