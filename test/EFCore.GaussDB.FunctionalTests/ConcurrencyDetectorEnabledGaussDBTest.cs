namespace Microsoft.EntityFrameworkCore;

public class ConcurrencyDetectorEnabledGaussDBTest : ConcurrencyDetectorEnabledRelationalTestBase<
    ConcurrencyDetectorEnabledGaussDBTest.ConcurrencyDetectorGaussDBFixture>
{
    public ConcurrencyDetectorEnabledGaussDBTest(ConcurrencyDetectorGaussDBFixture fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public override Task FromSql(bool async)
        => ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.FromSqlRaw("""
                    select * from "Products"
                    """).ToListAsync()
                : c.Products.FromSqlRaw("""
                    select * from "Products"
                    """).ToList());

    protected override async Task ConcurrencyDetectorTest(Func<ConcurrencyDetectorDbContext, Task<object>> test)
    {
        await base.ConcurrencyDetectorTest(test);

        Assert.Empty(Fixture.TestSqlLoggerFactory.SqlStatements);
    }

    public class ConcurrencyDetectorGaussDBFixture : ConcurrencyDetectorFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;
    }
}
