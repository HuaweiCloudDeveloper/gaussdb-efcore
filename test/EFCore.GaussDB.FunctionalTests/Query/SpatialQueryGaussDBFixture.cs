namespace Microsoft.EntityFrameworkCore.Query;

public class SpatialQueryGaussDBFixture : SpatialQueryRelationalFixture
{
    // We instruct the test store to pass a connection string to UseGaussDB() instead of a DbConnection - that's required to allow
    // EF's UseNodaTime() to function properly and instantiate an GaussDBDataSource internally.
    protected override ITestStoreFactory TestStoreFactory
        => new GaussDBTestStoreFactory(useConnectionString: true);

    protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
        => base.AddServices(serviceCollection).AddEntityFrameworkGaussDBNetTopologySuite();

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        modelBuilder.HasPostgresExtension("postgis");
    }

    // TODO: #1232
    // protected override bool CanExecuteQueryString => true;
}
