namespace Microsoft.EntityFrameworkCore;

public class GaussDBFixture : ServiceProviderFixtureBase
{
    public TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
