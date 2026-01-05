using System.Data.Common;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Diagnostics.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities.FakeProvider;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

#nullable enable

public class GaussDBRelationalConnectionTest
{
    [Fact]
    public void Creates_GaussDBConnection()
    {
        using var connection = CreateConnection();

        Assert.IsType<GaussDBConnection>(connection.DbConnection);
    }

    [Fact]
    public void Uses_DbDataSource_from_DbContextOptions()
    {
        using var dataSource = GaussDBDataSource.Create("Host=FakeHost");

        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddGaussDBDataSource("Host=FakeHost")
            // ReSharper disable once AccessToDisposedClosure
            .AddDbContext<FakeDbContext>(o => o.UseGaussDB(dataSource));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        var context1 = scope1.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Same(dataSource, relationalConnection1.DbDataSource);

        var connection1 = context1.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Host=FakeHost", connection1.ConnectionString);

        using var scope2 = serviceProvider.CreateScope();
        var context2 = scope2.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Same(dataSource, relationalConnection2.DbDataSource);

        var connection2 = context2.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Host=FakeHost", connection2.ConnectionString);
    }

    [Fact]
    public void Uses_DbDataSource_from_application_service_provider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddGaussDBDataSource("Host=FakeHost")
            .AddDbContext<FakeDbContext>(o => o.UseGaussDB());

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var dataSource = serviceProvider.GetRequiredService<GaussDBDataSource>();

        using var scope1 = serviceProvider.CreateScope();
        var context1 = scope1.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Same(dataSource, relationalConnection1.DbDataSource);

        var connection1 = context1.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Host=FakeHost", connection1.ConnectionString);

        using var scope2 = serviceProvider.CreateScope();
        var context2 = scope2.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Same(dataSource, relationalConnection2.DbDataSource);

        var connection2 = context2.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Host=FakeHost", connection2.ConnectionString);
    }

    [Fact] // #3060
    public void DbDataSource_from_application_service_provider_does_not_used_if_connection_string_is_specified()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddGaussDBDataSource("Host=FakeHost1")
            .AddDbContext<FakeDbContext>(o => o.UseGaussDB("Host=FakeHost2"));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        var context1 = scope1.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Null(relationalConnection1.DbDataSource);

        var connection1 = context1.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Host=FakeHost2", connection1.ConnectionString);
    }

    [Fact]
    public void Data_source_config_with_same_connection_string()
    {
        // The connection string is the same, so the same data source gets resolved.
        // This works well as long as ConfigureDataSource() has the same lambda.
        var context1 = new ConfigurableContext(
            "Host=FakeHost1", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App1"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1;Application Name=App1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext(
            "Host=FakeHost1", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App1"));
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1;Application Name=App1", connection1.ConnectionString);
        Assert.Same(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Data_source_config_with_different_connection_strings()
    {
        // When different connection strings are used, different data sources are created internally.
        var context1 = new ConfigurableContext(
            "Host=FakeHost1", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App1"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1;Application Name=App1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext(
            "Host=FakeHost2", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App2"));
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost2;Application Name=App2", connection2.ConnectionString);
        Assert.NotSame(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Data_source_config_with_same_connection_string_and_different_lambda()
    {
        // Bad case: same connection string but with a different data source config lambda.
        // Same data source gets reused, and so the differing data source config gets ignored.
        var context1 = new ConfigurableContext(
            "Host=FakeHost1", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App1"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1;Application Name=App1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext(
            "Host=FakeHost1", no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "App2"));
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        // Note the incorrect Application Name below, because the 1st data source was resolved based on the connection string only
        Assert.Equal("Host=FakeHost1;Application Name=App1", connection2.ConnectionString);
        Assert.Same(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Data_source_config_without_a_connection_string()
    {
        var context = new ConfigurableContext(
            connectionString: null,
            no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.Host = "192.168.1.1"));
        var connection1 = (NpgsqlRelationalConnection)context.GetService<IRelationalConnection>();
        Assert.Equal("Host=192.168.1.1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);
    }

    [Fact]
    public void Plugin_config_with_same_connection_string()
    {
        // The connection string and plugin config are the same, so the same data source gets resolved.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.UseNetTopologySuite());
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost1", no => no.UseNetTopologySuite());
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.Same(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Plugin_config_with_different_connection_strings()
    {
        // When different connection strings are used, different data sources are created internally.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.UseNetTopologySuite());
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost2", no => no.UseNetTopologySuite());
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost2", connection2.ConnectionString);
        Assert.NotSame(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Plugin_config_with_different_connection_strings_and_different_plugins()
    {
        // Since the plugin configuration is a singleton option, a different service provider gets resolved and we have different data
        // sources.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.UseNetTopologySuite());
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost1", no => no.UseNodaTime());
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection2.ConnectionString);
        Assert.NotSame(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Enum_config_with_same_connection_string()
    {
        // The connection string and plugin config are the same, so the same data source gets resolved.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.MapEnum<Mood>("mood"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost1", no => no.MapEnum<Mood>("mood"));
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.Same(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Enum_config_with_different_connection_strings()
    {
        // When different connection strings are used, different data sources are created internally.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.MapEnum<Mood>("mood"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost2", no => no.MapEnum<Mood>("mood"));
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost2", connection2.ConnectionString);
        Assert.NotSame(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Enum_config_with_different_connection_strings_and_different_enums()
    {
        // Since the enum configuration is a singleton option, a different service provider gets resolved, and we have different data
        // sources.
        var context1 = new ConfigurableContext("Host=FakeHost1", no => no.MapEnum<Mood>("mood"));
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.NotNull(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost1", _ => { /* no enums */});
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection2.ConnectionString);
        Assert.NotSame(connection1.DbDataSource, connection2.DbDataSource);
    }

    [Fact]
    public void Data_source_and_data_source_config_are_incompatible()
    {
        using var dataSource = GaussDBDataSource.Create("Host=FakeHost");

        var optionsBuilder = new DbContextOptionsBuilder<FakeDbContext>();
        optionsBuilder.UseGaussDB(dataSource, no => no.ConfigureDataSource(dsb => dsb.ConnectionStringBuilder.ApplicationName = "foo"));

        var context1 = new FakeDbContext(optionsBuilder.Options);
        var exception = Assert.Throws<NotSupportedException>(() => context1.GetService<IRelationalConnection>());
        Assert.Equal(GaussDBStrings.DataSourceAndConfigNotSupported, exception.Message);
    }

    [Fact]
    public void Multiple_connection_strings_without_data_source_features()
    {
        var context1 = new ConfigurableContext("Host=FakeHost1");
        var connection1 = (GaussDBRelationalConnection)context1.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection1.ConnectionString);
        Assert.Null(connection1.DbDataSource);

        var context2 = new ConfigurableContext("Host=FakeHost1");
        var connection2 = (GaussDBRelationalConnection)context2.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost1", connection2.ConnectionString);
        Assert.Null(connection2.DbDataSource);

        var context3 = new ConfigurableContext("Host=FakeHost2");
        var connection3 = (GaussDBRelationalConnection)context3.GetService<IRelationalConnection>();
        Assert.Equal("Host=FakeHost2", connection3.ConnectionString);
        Assert.Null(connection3.DbDataSource);
    }

    [Fact]
    public void Can_create_master_connection_with_connection_string()
    {
        using var connection = CreateConnection();
        using var master = connection.CreateAdminConnection();

        Assert.Equal(
            @"Host=localhost;Database=postgres;Username=some_user;Password=some_password;Pooling=False;Multiplexing=False",
            master.ConnectionString);
    }

    [Fact]
    public void Can_create_master_connection_with_connection_string_and_alternate_admin_db()
    {
        var options = new DbContextOptionsBuilder()
            .UseGaussDB(
                @"Host=localhost;Database=GaussDBConnectionTest;Username=some_user;Password=some_password",
                b => b.UseAdminDatabase("template0"))
            .Options;

        using var connection = CreateConnection(options);
        using var master = connection.CreateAdminConnection();

        Assert.Equal(
            @"Host=localhost;Database=template0;Username=some_user;Password=some_password;Pooling=False;Multiplexing=False",
            master.ConnectionString);
    }

    [Theory]
    [InlineData("false")]
    [InlineData("False")]
    [InlineData("FALSE")]
    public void CurrentAmbientTransaction_returns_null_with_enlist_set_to_false(string falseValue)
    {
        var options = new DbContextOptionsBuilder()
            .UseGaussDB(
                @"Host=localhost;Database=GaussDBConnectionTest;Username=some_user;Password=some_password;Enlist=" + falseValue)
            .Options;

        Transaction.Current = new CommittableTransaction();

        using var connection = CreateConnection(options);
        Assert.Null(connection.CurrentAmbientTransaction);

        Transaction.Current = null;
    }

    [Theory]
    [InlineData(";Enlist=true")]
    [InlineData("")] // Enlist is true by default
    public void CurrentAmbientTransaction_returns_transaction_with_enlist_enabled(string enlist)
    {
        var options = new DbContextOptionsBuilder()
            .UseGaussDB(
                @"Host=localhost;Database=GaussDBConnectionTest;Username=some_user;Password=some_password" + enlist)
            .Options;

        var transaction = new CommittableTransaction();
        Transaction.Current = transaction;

        using var connection = CreateConnection(options);
        Assert.Equal(transaction, connection.CurrentAmbientTransaction);

        Transaction.Current = null;
    }

    [ConditionalFact]
    public async Task CloneWith_with_connection_and_connection_string()
    {
        var services = GaussDBTestHelpers.Instance.CreateContextServices(
            new DbContextOptionsBuilder()
                .UseGaussDB("Host=localhost;Database=DummyDatabase")
                .Options);

        var relationalConnection = (GaussDBRelationalConnection)services.GetRequiredService<IRelationalConnection>();

        var clone = await relationalConnection.CloneWith("Host=localhost;Database=DummyDatabase;Application Name=foo", async: true);

        Assert.Equal("Host=localhost;Database=DummyDatabase;Application Name=foo", clone.ConnectionString);
    }

    public static GaussDBRelationalConnection CreateConnection(DbContextOptions? options = null, DbDataSource? dataSource = null)
    {
        options ??= new DbContextOptionsBuilder()
            .UseGaussDB(@"Host=localhost;Database=GaussDBConnectionTest;Username=some_user;Password=some_password")
            .Options;

        foreach (var extension in options.Extensions)
        {
            extension.Validate(options);
        }

        var dbContextOptions = CreateOptions();

        return new GaussDBRelationalConnection(
            new RelationalConnectionDependencies(
                options,
                new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new GaussDBLoggingDefinitions(),
                    new NullDbContextLogger()),
                new RelationalConnectionDiagnosticsLogger(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new GaussDBLoggingDefinitions(),
                    new NullDbContextLogger(),
                    dbContextOptions),
                new NamedConnectionStringResolver(options),
                new RelationalTransactionFactory(
                    new RelationalTransactionFactoryDependencies(
                        new RelationalSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()))),
                new CurrentDbContext(new FakeDbContext()),
                new RelationalCommandBuilderFactory(
                    new RelationalCommandBuilderDependencies(
                        new GaussDBTypeMappingSource(
                            TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                            TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
                            new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
                            new GaussDBSingletonOptions()),
                        new ExceptionDetector(),
                        new LoggingOptions())),
                new ExceptionDetector()),
            new GaussDBDataSourceManager([]),
            dbContextOptions);
    }

    private const string ConnectionString = "Fake Connection String";

    private static IDbContextOptions CreateOptions(RelationalOptionsExtension? optionsExtension = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(
                optionsExtension
                ?? new FakeRelationalOptionsExtension().WithConnectionString(ConnectionString));

        return optionsBuilder.Options;
    }

    private class FakeDbContext : DbContext
    {
        public FakeDbContext()
        {
        }

        public FakeDbContext(DbContextOptions<FakeDbContext> options)
            : base(options)
        {
        }
    }

    private class ConfigurableContext(string connectionString, Action<GaussDBDbContextOptionsBuilder>? npgsqlOptionsAction = null) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB(connectionString, npgsqlOptionsAction);
    }

    private enum Mood
    {
        // ReSharper disable once UnusedMember.Local
        Happy,
        // ReSharper disable once UnusedMember.Local
        Sad
    }
}
