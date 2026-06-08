using System.Globalization;
using Microsoft.Extensions.Configuration;

using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public static class TestEnvironment
{
    private static readonly Dictionary<string, bool> ExtensionAvailability = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, bool> InstalledExtensions = new(StringComparer.Ordinal);

    public static IConfiguration Config { get; }

    static TestEnvironment()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: true)
            .AddJsonFile("config.test.json", optional: true)
            .AddEnvironmentVariables();

        Config = configBuilder.Build()
            .GetSection("Test:GaussDB");

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }

    private const string DefaultConnectionString = "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Port=5432";

    public static string DefaultConnection
        => Config["DefaultConnection"] ?? DefaultConnectionString;

    public static bool EnableExtensionConnectionOption
        => !string.Equals(Config["EnableExtensionConnectionOption"], "false", StringComparison.OrdinalIgnoreCase);

    private static Version? _postgresVersion;

    public static Version PostgresVersion
    {
        get
        {
            if (_postgresVersion is not null)
            {
                return _postgresVersion;
            }

            using var conn = new GaussDBConnection(GaussDBTestStore.CreateConnectionString("postgres"));
            conn.Open();
            return _postgresVersion = conn.PostgreSqlVersion;
        }
    }

    private static bool? _isPostgisAvailable;
    private static bool? _isHstoreSupported;

    public static bool IsPostgisAvailable
    {
        get
        {
            if (_isPostgisAvailable.HasValue)
            {
                return _isPostgisAvailable.Value;
            }

            using var conn = new GaussDBConnection(GaussDBTestStore.CreateConnectionString("postgres"));
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM pg_available_extensions WHERE \"name\" = 'postgis' LIMIT 1)";
            _isPostgisAvailable = (bool)cmd.ExecuteScalar()!;
            return _isPostgisAvailable.Value;
        }
    }

    public static bool IsExtensionAvailable(string extensionName)
    {
        if (ExtensionAvailability.TryGetValue(extensionName, out var isAvailable))
        {
            return isAvailable;
        }

        using var conn = new GaussDBConnection(GaussDBTestStore.CreateConnectionString("postgres"));
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM pg_available_extensions WHERE \"name\" = @name LIMIT 1)";
        cmd.Parameters.AddWithValue("name", extensionName);

        isAvailable = (bool)cmd.ExecuteScalar()!;
        ExtensionAvailability[extensionName] = isAvailable;
        return isAvailable;
    }

    public static bool IsExtensionInstalled(string extensionName, string databaseName = "postgres")
    {
        var cacheKey = databaseName + ":" + extensionName;
        if (InstalledExtensions.TryGetValue(cacheKey, out var isInstalled))
        {
            return isInstalled;
        }

        using var conn = new GaussDBConnection(GaussDBTestStore.CreateConnectionString(databaseName));
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT EXISTS (SELECT 1 FROM pg_extension WHERE extname = @name LIMIT 1)";
        cmd.Parameters.AddWithValue("name", extensionName);

        isInstalled = (bool)cmd.ExecuteScalar()!;
        InstalledExtensions[cacheKey] = isInstalled;
        return isInstalled;
    }

    public static bool IsHstoreSupported
    {
        get
        {
            if (_isHstoreSupported.HasValue)
            {
                return _isHstoreSupported.Value;
            }

            if (!IsExtensionAvailable("hstore"))
            {
                _isHstoreSupported = false;
                return false;
            }

            using var conn = new GaussDBConnection(GaussDBTestStore.CreateConnectionString("postgres"));
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT 'a=>b'::hstore";

            try
            {
                _ = cmd.ExecuteScalar();
                _isHstoreSupported = true;
            }
            catch (PostgresException e) when (e.SqlState == "0A000")
            {
                _isHstoreSupported = false;
            }

            return _isHstoreSupported.Value;
        }
    }

    public static void ConfigureDataSourceCompatibility(GaussDBDbContextOptionsBuilder optionsBuilder)
    {
        if (IsExtensionAvailable("hstore") && !IsHstoreSupported)
        {
            optionsBuilder.ConfigureDataSource(
                dataSourceBuilder => dataSourceBuilder.ConfigureTypeLoading(
                    typeLoadingOptionsBuilder => typeLoadingOptionsBuilder.EnableTypeLoading(false)));
        }
    }
}
