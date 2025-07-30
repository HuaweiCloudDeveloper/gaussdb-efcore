namespace Microsoft.EntityFrameworkCore;

public class DataAnnotationGaussDBTest(DataAnnotationGaussDBTest.DataAnnotationGaussDBFixture fixture)
    : DataAnnotationRelationalTestBase<DataAnnotationGaussDBTest.DataAnnotationGaussDBFixture>(fixture)
{
    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    protected override TestHelpers TestHelpers
        => GaussDBTestHelpers.Instance;

    public override Task StringLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        => Task.CompletedTask; // GaussDB does not support length

    public override Task TimestampAttribute_throws_if_value_in_database_changed()
        => Task.CompletedTask; // GaussDB does not support length

    public override Task MaxLengthAttribute_throws_while_inserting_value_longer_than_max_length()
        => Task.CompletedTask; // GaussDB does not support length

    public class DataAnnotationGaussDBFixture : DataAnnotationRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
    }
}
