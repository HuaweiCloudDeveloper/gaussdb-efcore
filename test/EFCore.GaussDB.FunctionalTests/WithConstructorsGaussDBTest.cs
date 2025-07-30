namespace Microsoft.EntityFrameworkCore;

public class WithConstructorsGaussDBTest(WithConstructorsGaussDBTest.WithConstructorsGaussDBFixture fixture)
    : WithConstructorsTestBase<WithConstructorsGaussDBTest.WithConstructorsGaussDBFixture>(fixture)
{
    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public class WithConstructorsGaussDBFixture : WithConstructorsFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<BlogQuery>().HasNoKey().ToSqlQuery("""
                SELECT * FROM "Blog"
                """);
        }
    }
}
