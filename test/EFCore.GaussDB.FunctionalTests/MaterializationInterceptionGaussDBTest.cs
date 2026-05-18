namespace Microsoft.EntityFrameworkCore;

public class MaterializationInterceptionGaussDBTest :
    MaterializationInterceptionTestBase<MaterializationInterceptionGaussDBTest.GaussDBLibraryContext>
{
    public class GaussDBLibraryContext(DbContextOptions options) : LibraryContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings);

            // #2548
            // modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings, b => b.ToJson());
        }
    }

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
