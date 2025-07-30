using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

// ReSharper disable StringLiteralTypo
namespace Microsoft.EntityFrameworkCore;

#nullable disable

public class ConnectionSpecificationTest
{
    [Fact]
    public async Task Can_specify_connection_string_in_OnConfiguring()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<StringInOnConfiguringContext>()
            .BuildServiceProvider();

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = serviceProvider.GetRequiredService<StringInOnConfiguringContext>();

        Assert.True(await context.Customers.AnyAsync());
    }

    [Fact]
    public async Task Can_specify_connection_string_in_OnConfiguring_with_default_service_provider()
    {
        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = new StringInOnConfiguringContext();

        Assert.True(await context.Customers.AnyAsync());
    }

    private class StringInOnConfiguringContext : NorthwindContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB(GaussDBTestStore.NorthwindConnectionString, b => b.ApplyConfiguration());
    }

    [Fact]
    public async Task Can_specify_connection_in_OnConfiguring()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => new GaussDBConnection(GaussDBTestStore.NorthwindConnectionString))
            .AddDbContext<ConnectionInOnConfiguringContext>().BuildServiceProvider();

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = serviceProvider.GetRequiredService<ConnectionInOnConfiguringContext>();

        Assert.True(await context.Customers.AnyAsync());
    }

    [Fact]
    public async Task Can_specify_connection_in_OnConfiguring_with_default_service_provider()
    {
        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = new ConnectionInOnConfiguringContext(new GaussDBConnection(GaussDBTestStore.NorthwindConnectionString));

        Assert.True(await context.Customers.AnyAsync());
    }

    private class ConnectionInOnConfiguringContext(GaussDBConnection connection) : NorthwindContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB(connection, b => b.ApplyConfiguration());

        public override void Dispose()
        {
            connection.Dispose();
            base.Dispose();
        }
    }

    // ReSharper disable once UnusedMember.Local
    private class StringInConfigContext : NorthwindContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB("Database=Crunchie", b => b.ApplyConfiguration());
    }

    [Fact]
    public void Throws_if_no_connection_found_in_config_without_UseGaussDB()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<NoUseGaussDBContext>().BuildServiceProvider();

        using var context = serviceProvider.GetRequiredService<NoUseGaussDBContext>();

        Assert.Equal(
            CoreStrings.NoProviderConfigured,
            Assert.Throws<InvalidOperationException>(() => context.Customers.Any()).Message);
    }

    [Fact]
    public void Throws_if_no_config_without_UseGaussDB()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<NoUseGaussDBContext>().BuildServiceProvider();
        using var context = serviceProvider.GetRequiredService<NoUseGaussDBContext>();

        Assert.Equal(
            CoreStrings.NoProviderConfigured,
            Assert.Throws<InvalidOperationException>(() => context.Customers.Any()).Message);
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class NoUseGaussDBContext : NorthwindContextBase;

    [Fact]
    public async Task Can_depend_on_DbContextOptions()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => new GaussDBConnection(GaussDBTestStore.NorthwindConnectionString))
            .AddDbContext<OptionsContext>()
            .BuildServiceProvider();

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = serviceProvider.GetRequiredService<OptionsContext>();

        Assert.True(await context.Customers.AnyAsync());
    }

    [Fact]
    public async Task Can_depend_on_DbContextOptions_with_default_service_provider()
    {
        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = new OptionsContext(
            new DbContextOptions<OptionsContext>(),
            new GaussDBConnection(GaussDBTestStore.NorthwindConnectionString));

        Assert.True(await context.Customers.AnyAsync());
    }

    private class OptionsContext(DbContextOptions<OptionsContext> options, GaussDBConnection connection)
        : NorthwindContextBase(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assert.Same(options, optionsBuilder.Options);

            optionsBuilder.UseGaussDB(connection, b => b.ApplyConfiguration());

            Assert.NotSame(options, optionsBuilder.Options);
        }

        public override void Dispose()
        {
            connection.Dispose();
            base.Dispose();
        }
    }

    [Fact]
    public async Task Can_depend_on_non_generic_options_when_only_one_context()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<NonGenericOptionsContext>()
            .BuildServiceProvider();

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = serviceProvider.GetRequiredService<NonGenericOptionsContext>();

        Assert.True(await context.Customers.AnyAsync());
    }

    [Fact]
    public async Task Can_depend_on_non_generic_options_when_only_one_context_with_default_service_provider()
    {
        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();
        await using var context = new NonGenericOptionsContext(new DbContextOptions<DbContext>());

        Assert.True(await context.Customers.AnyAsync());
    }

    private class NonGenericOptionsContext(DbContextOptions options) : NorthwindContextBase(options)
    {
        private readonly DbContextOptions _options = options;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assert.Same(_options, optionsBuilder.Options);

            optionsBuilder.UseGaussDB(GaussDBTestStore.NorthwindConnectionString, b => b.ApplyConfiguration());

            Assert.NotSame(_options, optionsBuilder.Options);
        }
    }

    private class NorthwindContextBase : DbContext
    {
        protected NorthwindContextBase()
        {
        }

        protected NorthwindContextBase(DbContextOptions options)
            : base(options)
        {
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Customer>(
                b =>
                {
                    b.HasKey(c => c.CustomerId);
                    b.ToTable("Customers");
                });
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Customer
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string CustomerId { get; set; }

        // ReSharper disable once UnusedMember.Local
        public string CompanyName { get; set; }

        // ReSharper disable once UnusedMember.Local
        public string Fax { get; set; }
    }

    #region Added for GaussDB

    [Fact]
    public async Task Can_create_admin_connection_with_data_source()
    {
        await using var dataSource = GaussDBDataSource.Create(GaussDBTestStore.NorthwindConnectionString);

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();

        var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
        optionsBuilder.UseGaussDB(dataSource, b => b.ApplyConfiguration());
        await using var context = new GeneralOptionsContext(optionsBuilder.Options);

        var relationalConnection = context.GetService<IGaussDBRelationalConnection>();
        await using var adminConnection = relationalConnection.CreateAdminConnection();

        Assert.Equal("postgres", new GaussDBConnectionStringBuilder(adminConnection.ConnectionString).Database);

        await adminConnection.OpenAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Can_create_admin_connection_with_connection_string()
    {
        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();

        var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
        optionsBuilder.UseGaussDB(GaussDBTestStore.NorthwindConnectionString, b => b.ApplyConfiguration());
        await using var context = new GeneralOptionsContext(optionsBuilder.Options);

        var relationalConnection = context.GetService<IGaussDBRelationalConnection>();
        await using var adminConnection = relationalConnection.CreateAdminConnection();

        Assert.Equal("postgres", new GaussDBConnectionStringBuilder(adminConnection.ConnectionString).Database);

        await adminConnection.OpenAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Can_create_admin_connection_with_connection()
    {
        await using var connection = new GaussDBConnection(GaussDBTestStore.NorthwindConnectionString);
        connection.Open();

        await using var _ = await GaussDBTestStore.GetNorthwindStoreAsync();

        var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
        optionsBuilder.UseGaussDB(connection, b => b.ApplyConfiguration());
        await using var context = new GeneralOptionsContext(optionsBuilder.Options);

        var relationalConnection = context.GetService<IGaussDBRelationalConnection>();
        await using var adminConnection = relationalConnection.CreateAdminConnection();

        Assert.Equal("postgres", new GaussDBConnectionStringBuilder(adminConnection.ConnectionString).Database);

        adminConnection.Open();
    }

    private class GeneralOptionsContext(DbContextOptions<GeneralOptionsContext> options) : NorthwindContextBase(options);

    #endregion
}
