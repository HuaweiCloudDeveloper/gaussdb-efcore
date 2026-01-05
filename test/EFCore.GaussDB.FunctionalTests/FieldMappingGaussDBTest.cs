namespace Microsoft.EntityFrameworkCore;

public class FieldMappingGaussDBTest(FieldMappingGaussDBTest.FieldMappingGaussDBFixture fixture)
    : FieldMappingTestBase<FieldMappingGaussDBTest.FieldMappingGaussDBFixture>(fixture)
{
    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public class FieldMappingGaussDBFixture : FieldMappingFixtureBase
    {
        protected override string StoreName { get; } = "FieldMapping";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
