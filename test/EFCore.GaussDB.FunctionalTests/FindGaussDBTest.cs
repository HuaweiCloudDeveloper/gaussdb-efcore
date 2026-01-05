namespace Microsoft.EntityFrameworkCore;

public abstract class FindGaussDBTest : FindTestBase<FindGaussDBTest.FindGaussDBFixture>
{
    protected FindGaussDBTest(FindGaussDBFixture fixture)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
    }

    public class FindGaussDBTestSet(FindGaussDBFixture fixture) : FindGaussDBTest(fixture)
    {
        protected override TestFinder Finder { get; } = new FindViaSetFinder();
    }

    public class FindGaussDBTestContext(FindGaussDBFixture fixture) : FindGaussDBTest(fixture)
    {
        protected override TestFinder Finder { get; } = new FindViaContextFinder();
    }

    public class FindGaussDBTestNonGeneric(FindGaussDBFixture fixture) : FindGaussDBTest(fixture)
    {
        protected override TestFinder Finder { get; } = new FindViaNonGenericContextFinder();
    }

    public class FindGaussDBFixture : FindFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
