using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;

namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsQueryGaussDBFixture : ComplexNavigationsQueryRelationalFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
        // supported.
        modelBuilder.Entity<Level1>().Property(l => l.Date).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Level2>().Property(l => l.Date).HasColumnType("timestamp without time zone");
    }
    // public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    // {
    //     var optionsBuilder = base.AddOptions(builder);
    //     new GaussDBDbContextOptionsBuilder(optionsBuilder).ReverseNullOrdering();
    //     return optionsBuilder;
    // }
}
