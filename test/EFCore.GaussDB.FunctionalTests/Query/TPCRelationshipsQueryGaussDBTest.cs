namespace Microsoft.EntityFrameworkCore.Query;

public class TPCRelationshipsQueryGaussDBTest
    : TPCRelationshipsQueryTestBase<TPCRelationshipsQueryGaussDBTest.TPCRelationshipsQueryGaussDBFixture>
{
    public TPCRelationshipsQueryGaussDBTest(
        TPCRelationshipsQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
    }

    public class TPCRelationshipsQueryGaussDBFixture : TPCRelationshipsQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
