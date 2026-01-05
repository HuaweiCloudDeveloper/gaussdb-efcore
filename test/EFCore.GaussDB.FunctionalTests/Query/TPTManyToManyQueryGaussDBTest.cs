namespace Microsoft.EntityFrameworkCore.Query;

public class TPTManyToManyQueryGaussDBTest : TPTManyToManyQueryRelationalTestBase<TPTManyToManyQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public TPTManyToManyQueryGaussDBTest(TPTManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // TODO: #1232
    // protected override bool CanExecuteQueryString => true;
}
