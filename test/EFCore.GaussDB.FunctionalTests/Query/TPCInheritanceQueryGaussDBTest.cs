namespace Microsoft.EntityFrameworkCore.Query;

public class TPCInheritanceQueryGaussDBTest(TPCInheritanceQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
    : TPCInheritanceQueryTestBase<TPCInheritanceQueryGaussDBFixture>(fixture, testOutputHelper)
{
    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());
}
