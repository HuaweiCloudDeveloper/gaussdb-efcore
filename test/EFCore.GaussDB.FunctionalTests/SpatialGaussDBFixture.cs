using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public class SpatialGaussDBFixture : SpatialFixtureBase
{
    // We instruct the test store to pass a connection string to UseGaussDB() instead of a DbConnection - that's required to allow
    // EF's UseNetTopologySuite() to function properly and instantiate an GaussDBDataSource internally.
    protected override ITestStoreFactory TestStoreFactory
        => new GaussDBTestStoreFactory(useConnectionString: true);

    protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
        => base.AddServices(serviceCollection)
            .AddEntityFrameworkGaussDBNetTopologySuite();

    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        var optionsBuilder = base.AddOptions(builder);
        new GaussDBDbContextOptionsBuilder(optionsBuilder)
            .UseNetTopologySuite()
            .SetPostgresVersion(TestEnvironment.PostgresVersion);

        return optionsBuilder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        modelBuilder.HasPostgresExtension("uuid-ossp");
    }
}
