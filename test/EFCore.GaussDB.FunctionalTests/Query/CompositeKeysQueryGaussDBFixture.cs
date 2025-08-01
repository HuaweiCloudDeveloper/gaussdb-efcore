using Microsoft.EntityFrameworkCore.TestModels.CompositeKeysModel;

namespace Microsoft.EntityFrameworkCore.Query;

public class CompositeKeysQueryGaussDBFixture : CompositeKeysQueryRelationalFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
        // supported.
        modelBuilder.Entity<CompositeOne>().Property(c => c.Date).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<CompositeTwo>().Property(c => c.Date).HasColumnType("timestamp without time zone");
    }
}
