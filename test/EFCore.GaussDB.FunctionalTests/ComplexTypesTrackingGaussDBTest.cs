namespace Microsoft.EntityFrameworkCore;

public class ComplexTypesTrackingGaussDBTest : ComplexTypesTrackingTestBase<ComplexTypesTrackingGaussDBTest.GaussDBFixture>
{
    public ComplexTypesTrackingGaussDBTest(GaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public class GaussDBFixture : FixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;
    }
}
