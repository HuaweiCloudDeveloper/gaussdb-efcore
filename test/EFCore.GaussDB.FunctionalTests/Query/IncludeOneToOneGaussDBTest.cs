namespace Microsoft.EntityFrameworkCore.Query;

// ReSharper disable once UnusedMember.Global
public class IncludeOneToOneGaussDBTest : IncludeOneToOneTestBase<IncludeOneToOneGaussDBTest.OneToOneQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public IncludeOneToOneGaussDBTest(OneToOneQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public class OneToOneQueryGaussDBFixture : OneToOneQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
    }
}
