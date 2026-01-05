namespace Microsoft.EntityFrameworkCore;

// ReSharper disable once UnusedMember.Global
public class LoadGaussDBTest : LoadTestBase<LoadGaussDBTest.LoadGaussDBFixture>
{
    public LoadGaussDBTest(LoadGaussDBFixture fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    protected override void RecordLog()
        => Sql = Fixture.TestSqlLoggerFactory.Sql;

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string Sql { get; set; } = null!;

    public class LoadGaussDBFixture : LoadFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
