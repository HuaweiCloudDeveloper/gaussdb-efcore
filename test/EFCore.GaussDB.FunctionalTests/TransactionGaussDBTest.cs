using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore;

public class TransactionGaussDBTest(TransactionGaussDBTest.TransactionGaussDBFixture fixture)
    : TransactionTestBase<TransactionGaussDBTest.TransactionGaussDBFixture>(fixture)
{
    public override Task SaveChanges_can_be_used_with_AutoTransactionBehavior_Never(bool async)
        // GaussDB batches the inserts, creating an implicit transaction which fails the test
        // (see https://github.com/npgsql/npgsql/issues/1307)
        => Task.CompletedTask;

#pragma warning disable CS0618 // AutoTransactionsEnabled is obsolete
    public override Task SaveChanges_can_be_used_with_AutoTransactionsEnabled_false(bool async)
        // GaussDB batches the inserts, creating an implicit transaction which fails the test
        // (see https://github.com/npgsql/npgsql/issues/1307)
        => Task.CompletedTask;
#pragma warning restore CS0618

    protected override DbContext CreateContextWithConnectionString()
    {
        var options = Fixture.AddOptions(
                new DbContextOptionsBuilder()
                    .UseGaussDB(
                        TestStore.ConnectionString,
                        b => b.ApplyConfiguration()
                            .ExecutionStrategy(c => new GaussDBExecutionStrategy(c))
                            .ReverseNullOrdering()))
            .UseInternalServiceProvider(Fixture.ServiceProvider);

        return new DbContext(options.Options);
    }

    // In GaussDB, once the transaction enters the failed state it is always rolled back completely,
    // so none of the inserts are left.
    public override async Task SaveChanges_can_be_used_with_no_savepoint(bool async)
    {
        await using (var context = CreateContext())
        {
            context.Database.AutoSavepointsEnabled = false;

            await using var transaction = async
                ? await context.Database.BeginTransactionAsync()
                : context.Database.BeginTransaction();

            context.Add(new TransactionCustomer { Id = 77, Name = "Bobble" });

            if (async)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                context.SaveChanges();
            }

            context.Add(new TransactionCustomer { Id = 78, Name = "Hobble" });
            context.Add(new TransactionCustomer { Id = 1, Name = "Gobble" }); // Cause SaveChanges failure

            if (async)
            {
                await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
                await transaction.CommitAsync();
            }
            else
            {
                Assert.Throws<DbUpdateException>(() => context.SaveChanges());
                transaction.Commit();
            }

            context.Database.AutoSavepointsEnabled = true;
        }

        await using (var context = CreateContext())
        {
            Assert.Equal(2, context.Set<TransactionCustomer>().Max(c => c.Id));
        }
    }

    // Test generates an exception (by double-releasing the savepoint), which causes the transaction to enter
    // a failed state and roll back all changes.
    public override Task Savepoint_can_be_released(bool async)
        => Task.CompletedTask;

    protected override bool AmbientTransactionsSupported
        => true;

    protected override bool SnapshotSupported
        => true;

    protected override bool DirtyReadsOccur
        => false;

    public class TransactionGaussDBFixture : TransactionFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            new GaussDBDbContextOptionsBuilder(
                    base.AddOptions(builder))
                .ExecutionStrategy(c => new GaussDBExecutionStrategy(c));
            return builder;
        }
    }
}
