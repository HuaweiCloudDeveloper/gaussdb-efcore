namespace Microsoft.EntityFrameworkCore.Query;

public class TPTRelationshipsQueryGaussDBTest
    : TPTRelationshipsQueryTestBase<TPTRelationshipsQueryGaussDBTest.TPTRelationshipsQueryGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public TPTRelationshipsQueryGaussDBTest(
        TPTRelationshipsQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
    }

    public class TPTRelationshipsQueryGaussDBFixture : TPTRelationshipsQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
