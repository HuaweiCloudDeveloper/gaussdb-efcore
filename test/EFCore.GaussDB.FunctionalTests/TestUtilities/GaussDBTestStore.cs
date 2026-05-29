using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBTestStore : RelationalTestStore
{
    private const string InternalSchemas =
        "'pg_catalog', 'information_schema', 'sys', 'db4ai', 'dbe_perf', 'dbe_pldeveloper', 'dbe_profiler'";

    private readonly string? _scriptPath;
    private readonly string? _additionalSql;

    private const string Northwind = "Northwind";
    private static readonly Regex ScriptBatchSeparator =
        new("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0));
    private static readonly HashSet<string> NonHashDistributableKeyColumnTypes =
        new(StringComparer.OrdinalIgnoreCase) { "bytea" };

    public const int CommandTimeout = 600;

    public static readonly string NorthwindConnectionString = CreateConnectionString(Northwind);

    public static async Task<GaussDBTestStore> GetNorthwindStoreAsync()
        => (GaussDBTestStore)await GaussDBNorthwindTestStoreFactory.Instance
            .GetOrCreate(GaussDBNorthwindTestStoreFactory.Name).InitializeAsync(null, (Func<DbContext>?)null);

    public static Task<GaussDBTestStore> GetOrCreateInitializedAsync(string name)
        => new GaussDBTestStore(name).InitializeGaussDBAsync(null, (Func<DbContext>?)null, null);

    public static GaussDBTestStore GetOrCreate(
        string name,
        string? scriptPath = null,
        string? additionalSql = null,
        string? connectionStringOptions = null)
        => new(name, scriptPath, additionalSql, connectionStringOptions);

    public static GaussDBTestStore Create(string name, string? connectionStringOptions = null)
        => new(name, connectionStringOptions: connectionStringOptions, shared: false);

    public static Task<GaussDBTestStore> CreateInitializedAsync(string name)
        => new GaussDBTestStore(name, shared: false).InitializeGaussDBAsync(null, (Func<DbContext>?)null, null);

    public GaussDBTestStore(
        string name,
        string? scriptPath = null,
        string? additionalSql = null,
        string? connectionStringOptions = null,
        bool shared = true)
        : base(name, shared, CreateConnection(name, connectionStringOptions))
    {
        Name = name;

        if (scriptPath is not null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _scriptPath = Path.Combine(Path.GetDirectoryName(typeof(GaussDBTestStore).GetTypeInfo().Assembly.Location)!, scriptPath);
        }

        _additionalSql = additionalSql;
    }

    private static GaussDBConnection CreateConnection(string name, string? connectionStringOptions)
        => new(CreateConnectionString(name, connectionStringOptions));

    // ReSharper disable once MemberCanBePrivate.Global
    public async Task<GaussDBTestStore> InitializeGaussDBAsync(
        IServiceProvider? serviceProvider,
        Func<DbContext>? createContext,
        Func<DbContext, Task>? seed)
        => (GaussDBTestStore)await InitializeAsync(serviceProvider, createContext, seed);

    // ReSharper disable once UnusedMember.Global
    public async Task<GaussDBTestStore> InitializeGaussDBAsync(
        IServiceProvider serviceProvider,
        Func<GaussDBTestStore, DbContext> createContext,
        Func<DbContext, Task> seed)
        => await InitializeGaussDBAsync(serviceProvider, () => createContext(this), seed);

    protected override async Task InitializeAsync(Func<DbContext> createContext, Func<DbContext, Task>? seed, Func<DbContext, Task>? clean)
    {
        if (await CreateDatabaseAsync(clean))
        {
            if (_scriptPath is not null)
            {
                ExecuteScript(_scriptPath);

                if (_additionalSql is not null)
                {
                    Execute(Connection, command => command.ExecuteNonQuery(), _additionalSql);
                }
            }
            else
            {
                await using var context = createContext();
                if (TestEnvironment.IsDistributed)
                {
                    await EnsureUserTablesCreatedAsync(context);
                }
                else
                {
                    await context.Database.EnsureCreatedResilientlyAsync();
                    await EnsureUserTablesCreatedAsync(context);
                }

                if (_additionalSql is not null)
                {
                    Execute(Connection, command => command.ExecuteNonQuery(), _additionalSql);
                }

                if (seed is not null)
                {
                    await seed(context);
                }
            }
        }
    }

    public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
    {
        Action<GaussDBDbContextOptionsBuilder> npgsqlOptionsBuilder = b => b.ApplyConfiguration()
            .CommandTimeout(CommandTimeout)
            // The tests are written with the assumption that NULLs are sorted first (SQL Server and .NET behavior), but GaussDB
            // sorts NULLs last by default. This configures the provider to emit NULLS FIRST.
            .ReverseNullOrdering();

        return UseConnectionString
            ? builder.UseGaussDB(ConnectionString, npgsqlOptionsBuilder)
            : builder.UseGaussDB(Connection, npgsqlOptionsBuilder);
    }

    public static void EnsureCreatedWithUserTables(DbContext context)
    {
        if (TestEnvironment.IsDistributed)
        {
            EnsureCreatedWithUserTablesWithoutForeignKeys(context);
            return;
        }

        context.Database.EnsureCreatedResiliently();
        EnsureUserTablesCreated(context);
    }

    public static async Task EnsureCreatedWithUserTablesAsync(DbContext context)
    {
        if (TestEnvironment.IsDistributed)
        {
            await EnsureCreatedWithUserTablesWithoutForeignKeysAsync(context);
            return;
        }

        await context.Database.EnsureCreatedResilientlyAsync();
        await EnsureUserTablesCreatedAsync(context);
    }

    private async Task<bool> CreateDatabaseAsync(Func<DbContext, Task>? clean)
    {
        await using var master = new GaussDBConnection(CreateAdminConnectionString());

        if (await DatabaseExistsAsync(Name))
        {
            if (_scriptPath is not null
                && await ScriptDatabaseIsInitializedAsync())
            {
                return false;
            }

            await DropDatabaseAsync(master, Name);
            GaussDBConnection.ClearAllPools();
        }

        try
        {
            await ExecuteNonQueryAsync(master, GetCreateDatabaseStatement(Name));
        }
        catch (PostgresException e) when (e.SqlState == "23505")
        {
            await WaitForExistsAsync((GaussDBConnection)Connection);
            return false;
        }

        await WaitForExistsAsync((GaussDBConnection)Connection);

        return true;
    }

    private static async Task EnsureUserTablesCreatedAsync(DbContext context)
    {
        if (await HasUserTablesAsync(context.Database.GetDbConnection()))
        {
            return;
        }

        if (TestEnvironment.IsDistributed)
        {
            await CreateTablesWithoutForeignKeysAsync(context);
            return;
        }

        var creator = context.GetService<IRelationalDatabaseCreator>();
        await creator.CreateTablesAsync();
    }

    private static void EnsureUserTablesCreated(DbContext context)
    {
        if (HasUserTables(context.Database.GetDbConnection()))
        {
            return;
        }

        if (TestEnvironment.IsDistributed)
        {
            CreateTablesWithoutForeignKeys(context);
            return;
        }

        var creator = context.GetService<IRelationalDatabaseCreator>();
        creator.CreateTables();
    }

    private static async Task EnsureCreatedWithUserTablesWithoutForeignKeysAsync(DbContext context)
    {
        var creator = context.GetService<IRelationalDatabaseCreator>();

        if (!await creator.ExistsAsync())
        {
            await creator.CreateAsync();
        }

        if (!await HasUserTablesAsync(context.Database.GetDbConnection()))
        {
            await CreateTablesWithoutForeignKeysAsync(context);
        }
    }

    private static void EnsureCreatedWithUserTablesWithoutForeignKeys(DbContext context)
    {
        var creator = context.GetService<IRelationalDatabaseCreator>();

        if (!creator.Exists())
        {
            creator.Create();
        }

        if (!HasUserTables(context.Database.GetDbConnection()))
        {
            CreateTablesWithoutForeignKeys(context);
        }
    }

    private static IReadOnlyList<MigrationOperation> CreateTableOperations(
        DbContext context,
        IModel designTimeModel,
        bool omitForeignKeys)
    {
        var operations = context.GetService<IMigrationsModelDiffer>()
            .GetDifferences(null, designTimeModel.GetRelationalModel())
            .ToList();

        if (omitForeignKeys)
        {
            RemoveForeignKeys(operations);
        }

        return operations;
    }

    private static void RemoveForeignKeys(List<MigrationOperation> operations)
    {
        var foreignKeyIndexKeys = new HashSet<string>(StringComparer.Ordinal);

        foreach (var createTableOperation in operations.OfType<CreateTableOperation>())
        {
            foreach (var foreignKey in createTableOperation.ForeignKeys)
            {
                foreignKeyIndexKeys.Add(CreateIndexKey(createTableOperation.Schema, createTableOperation.Name, foreignKey.Columns));
            }

            createTableOperation.ForeignKeys.Clear();
        }

        foreach (var foreignKey in operations.OfType<AddForeignKeyOperation>())
        {
            foreignKeyIndexKeys.Add(CreateIndexKey(foreignKey.Schema, foreignKey.Table, foreignKey.Columns));
        }

        operations.RemoveAll(o => o is AddForeignKeyOperation);
        operations.RemoveAll(o => o is CreateIndexOperation index && foreignKeyIndexKeys.Contains(
            CreateIndexKey(index.Schema, index.Table, index.Columns)));
    }

    private static string CreateIndexKey(string? schema, string table, IReadOnlyList<string> columns)
        => $"{schema ?? ""}.{table}:{string.Join(",", columns)}";

    private static void CreateTablesWithoutForeignKeys(DbContext context)
    {
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var operations = CreateTableOperations(context, designTimeModel, omitForeignKeys: true);
        var commands = context.GetService<IMigrationsSqlGenerator>().Generate(operations, designTimeModel);
        var replicatedTables = GetReplicationDistributedTables(context, operations);

        try
        {
            if (replicatedTables.Count == 0)
            {
                context.GetService<IMigrationCommandExecutor>()
                    .ExecuteNonQuery(commands, context.GetService<IRelationalConnection>());
            }
            else
            {
                ExecuteDistributedMigrationCommands(context, commands, replicatedTables);
            }
        }
        catch (PostgresException e) when (e is { SqlState: "23505", ConstraintName: "pg_type_typname_nsp_index" })
        {
            // This occurs when two connections are trying to create the same database concurrently.
        }

        ReloadTypesIfNeeded(context, operations);
    }

    private static async Task CreateTablesWithoutForeignKeysAsync(DbContext context)
    {
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var operations = CreateTableOperations(context, designTimeModel, omitForeignKeys: true);
        var commands = context.GetService<IMigrationsSqlGenerator>().Generate(operations, designTimeModel);
        var replicatedTables = GetReplicationDistributedTables(context, operations);

        try
        {
            if (replicatedTables.Count == 0)
            {
                await context.GetService<IMigrationCommandExecutor>()
                    .ExecuteNonQueryAsync(commands, context.GetService<IRelationalConnection>());
            }
            else
            {
                await ExecuteDistributedMigrationCommandsAsync(context, commands, replicatedTables);
            }
        }
        catch (PostgresException e) when (e is { SqlState: "23505", ConstraintName: "pg_type_typname_nsp_index" })
        {
            // This occurs when two connections are trying to create the same database concurrently.
        }

        await ReloadTypesIfNeededAsync(context, operations);
    }

    private static ISet<string> GetReplicationDistributedTables(
        DbContext context,
        IEnumerable<MigrationOperation> operations)
    {
        var sqlGenerationHelper = context.GetService<ISqlGenerationHelper>();

        // Keep distributed setup from failing on table/index DDL that is unrelated to the behavior under test.
        return operations.OfType<CreateTableOperation>()
            .Where(RequiresReplicationDistribution)
            .Select(o => sqlGenerationHelper.DelimitIdentifier(o.Name, o.Schema))
            .Concat(operations.OfType<CreateIndexOperation>()
                .Where(o => o.IsUnique)
                .Select(o => sqlGenerationHelper.DelimitIdentifier(o.Table, o.Schema)))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static bool RequiresReplicationDistribution(CreateTableOperation operation)
        => operation.UniqueConstraints.Count > 0
            || operation.PrimaryKey?.Columns.Any(columnName =>
            {
                var column = operation.Columns.FirstOrDefault(c => c.Name == columnName);

                return column is not null
                    && IsNonHashDistributableKeyColumnType(column.ColumnType);
            }) == true;

    private static bool IsNonHashDistributableKeyColumnType(string? storeType)
    {
        if (string.IsNullOrWhiteSpace(storeType))
        {
            return false;
        }

        var normalizedStoreType = storeType.Trim();
        var parenthesisIndex = normalizedStoreType.IndexOf('(');
        if (parenthesisIndex >= 0)
        {
            normalizedStoreType = normalizedStoreType[..parenthesisIndex].TrimEnd();
        }

        return NonHashDistributableKeyColumnTypes.Contains(normalizedStoreType);
    }

    private static void ExecuteDistributedMigrationCommands(
        DbContext context,
        IReadOnlyList<MigrationCommand> commands,
        ISet<string> replicatedTables)
    {
        foreach (var command in commands)
        {
            ExecuteNonQuery(
                context.Database.GetDbConnection(),
                AddReplicationDistribution(command.CommandText, replicatedTables));
        }
    }

    private static async Task ExecuteDistributedMigrationCommandsAsync(
        DbContext context,
        IReadOnlyList<MigrationCommand> commands,
        ISet<string> replicatedTables)
    {
        foreach (var command in commands)
        {
            await ExecuteNonQueryAsync(
                context.Database.GetDbConnection(),
                AddReplicationDistribution(command.CommandText, replicatedTables));
        }
    }

    private static string AddReplicationDistribution(string commandText, ISet<string> replicatedTables)
    {
        if (!TryGetCreateTableTerminator(commandText, replicatedTables, out var terminatorIndex))
        {
            return commandText;
        }

        var createTableSql = commandText[..terminatorIndex].TrimEnd();
        if (createTableSql.Contains("DISTRIBUTE BY", StringComparison.OrdinalIgnoreCase))
        {
            return commandText;
        }

        return createTableSql + Environment.NewLine + "DISTRIBUTE BY REPLICATION" + commandText[terminatorIndex..];
    }

    private static bool TryGetCreateTableTerminator(
        string commandText,
        ISet<string> replicatedTables,
        out int terminatorIndex)
    {
        terminatorIndex = -1;
        var trimmedCommandText = commandText.TrimStart();

        foreach (var table in replicatedTables)
        {
            if (trimmedCommandText.StartsWith("CREATE TABLE " + table, StringComparison.OrdinalIgnoreCase)
                || trimmedCommandText.StartsWith("CREATE UNLOGGED TABLE " + table, StringComparison.OrdinalIgnoreCase))
            {
                terminatorIndex = commandText.IndexOf(';');
                return terminatorIndex >= 0;
            }
        }

        return false;
    }

    private static void ReloadTypesIfNeeded(DbContext context, IReadOnlyList<MigrationOperation> operations)
    {
        if (!RequiresTypeReload(operations)
            || context.Database.GetDbConnection() is not GaussDBConnection connection)
        {
            return;
        }

        connection.Open();
        try
        {
            connection.ReloadTypes();
        }
        finally
        {
            connection.Close();
        }
    }

    private static async Task ReloadTypesIfNeededAsync(DbContext context, IReadOnlyList<MigrationOperation> operations)
    {
        if (!RequiresTypeReload(operations)
            || context.Database.GetDbConnection() is not GaussDBConnection connection)
        {
            return;
        }

        await connection.OpenAsync();
        try
        {
            await connection.ReloadTypesAsync();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static bool RequiresTypeReload(IEnumerable<MigrationOperation> operations)
        => operations.OfType<AlterDatabaseOperation>()
            .Any(o => o.GetPostgresExtensions().Any() || o.GetPostgresEnums().Any() || o.GetPostgresRanges().Any());

    private async Task<bool> ScriptDatabaseIsInitializedAsync()
    {
        if (_scriptPath is null)
        {
            return true;
        }

        if (Name == Northwind)
        {
            return await ExecuteScalarAsync<long>(
                    Connection,
                    """
SELECT COUNT(*)
FROM pg_tables
WHERE schemaname = 'public' AND tablename = 'Customers'
""")
                > 0;
        }

        return await ExecuteScalarAsync<long>(
                Connection,
                $"""
SELECT COUNT(*)
FROM pg_tables
WHERE schemaname NOT IN ({InternalSchemas})
""")
            > 0;
    }

    private static async Task<bool> HasUserTablesAsync(DbConnection connection)
        => await ExecuteScalarAsync<long>(
                connection,
                $"""
SELECT COUNT(*)
FROM pg_tables
WHERE schemaname NOT IN ({InternalSchemas})
""")
            > 0;

    private static bool HasUserTables(DbConnection connection)
        => ExecuteScalar<long>(
                connection,
                $"""
SELECT COUNT(*)
FROM pg_tables
WHERE schemaname NOT IN ({InternalSchemas})
""")
            > 0;

    private static async Task WaitForExistsAsync(GaussDBConnection connection)
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync();
                }

                GaussDBConnection.ClearPool(connection);

                await connection.OpenAsync();
                await connection.CloseAsync();
                return;
            }
            catch (PostgresException e)
            {
                if (++retryCount >= 30
                    || e.SqlState != "08001" && e.SqlState != "08000" && e.SqlState != "08006")
                {
                    throw;
                }

                await Task.Delay(100);
            }
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void ExecuteScript(string scriptPath)
    {
        var script = File.ReadAllText(scriptPath);
        Execute(
            Connection, command =>
            {
                foreach (var batch in CreateScriptBatches(script, TestEnvironment.IsDistributed))
                {
                    command.CommandText = batch;
                    command.ExecuteNonQuery();
                }

                return 0;
            }, "");
    }

    private static IEnumerable<string> CreateScriptBatches(string script, bool omitForeignKeys)
        => ScriptBatchSeparator.Split(script)
            .Select(batch => omitForeignKeys ? OmitForeignKeysFromScriptBatch(batch) : batch)
            .Where(batch => !string.IsNullOrWhiteSpace(batch));

    private static string OmitForeignKeysFromScriptBatch(string batch)
    {
        var trimmedBatch = batch.TrimStart();
        if (trimmedBatch.StartsWith("ALTER TABLE", StringComparison.OrdinalIgnoreCase)
            && batch.Contains("ADD CONSTRAINT", StringComparison.OrdinalIgnoreCase)
            && batch.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
            && batch.Contains("REFERENCES", StringComparison.OrdinalIgnoreCase))
        {
            return "";
        }

        if (!batch.Contains("CREATE TABLE", StringComparison.OrdinalIgnoreCase)
            || !batch.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
        {
            return batch;
        }

        return OmitInlineForeignKeysFromCreateTable(batch);
    }

    private static string OmitInlineForeignKeysFromCreateTable(string batch)
    {
        var lines = batch.Replace("\r\n", "\n").Split('\n').ToList();
        var keptLines = new List<string>(lines.Count);
        var skippingForeignKey = false;
        var seenReferences = false;

        foreach (var line in lines)
        {
            if (!skippingForeignKey
                && line.Contains("CONSTRAINT", StringComparison.OrdinalIgnoreCase)
                && line.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
            {
                skippingForeignKey = true;
                seenReferences = line.Contains("REFERENCES", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (skippingForeignKey)
            {
                seenReferences = seenReferences || line.Contains("REFERENCES", StringComparison.OrdinalIgnoreCase);
                if (seenReferences
                    && Regex.IsMatch(line.Trim(), @"^\),?$", RegexOptions.None, TimeSpan.FromMilliseconds(1000.0)))
                {
                    skippingForeignKey = false;
                    seenReferences = false;
                }

                continue;
            }

            keptLines.Add(line);
        }

        RemoveDanglingCommasBeforeClosingParentheses(keptLines);

        return string.Join(Environment.NewLine, keptLines);
    }

    private static void RemoveDanglingCommasBeforeClosingParentheses(List<string> lines)
    {
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim() != ")")
            {
                continue;
            }

            for (var j = i - 1; j >= 0; j--)
            {
                if (string.IsNullOrWhiteSpace(lines[j]))
                {
                    continue;
                }

                if (lines[j].TrimEnd().EndsWith(",", StringComparison.Ordinal))
                {
                    var commaIndex = lines[j].LastIndexOf(',');
                    lines[j] = lines[j].Remove(commaIndex, 1);
                }

                break;
            }
        }
    }

    private static string GetCreateDatabaseStatement(string name)
        => $"""
            CREATE DATABASE "{name}"
            """;

    private static async Task<bool> DatabaseExistsAsync(string name)
    {
        await using var master = new GaussDBConnection(CreateAdminConnectionString());

        return await ExecuteScalarAsync<long>(master, $@"SELECT COUNT(*) FROM pg_database WHERE datname = '{name}'") > 0;
    }

    public async Task DeleteDatabaseAsync()
    {
        if (!await DatabaseExistsAsync(Name))
        {
            return;
        }

        await using var master = new GaussDBConnection(CreateAdminConnectionString());

        await DropDatabaseAsync(master, Name);

        GaussDBConnection.ClearAllPools();
    }

    // openGauss exposes a provider-specific command for forcibly clearing sessions.
    private static string GetDisconnectDatabaseSql(string name)
        => $"""
            CLEAN CONNECTION TO ALL FORCE FOR DATABASE "{name}";
""";

    private static string GetDropDatabaseSql(string name)
        => $"""
            DROP DATABASE "{name}"
            """;

    private static async Task DropDatabaseAsync(DbConnection connection, string name)
    {
        for (var retryCount = 0; ; retryCount++)
        {
            try
            {
                await ExecuteNonQueryAsync(connection, GetDisconnectDatabaseSql(name));
            }
            catch (PostgresException e) when (e.SqlState == "3D000")
            {
                return;
            }

            try
            {
                await ExecuteNonQueryAsync(connection, GetDropDatabaseSql(name));
                return;
            }
            catch (PostgresException e) when (e.SqlState == "3D000")
            {
                return;
            }
            catch (PostgresException e) when (e.SqlState == "55006" && retryCount < 30)
            {
                await Task.Delay(100);
            }
        }
    }

    public override void OpenConnection()
        => Connection.Open();

    public override Task OpenConnectionAsync()
        => Connection.OpenAsync();

    // ReSharper disable once UnusedMember.Global
    public T ExecuteScalar<T>(string sql, params object[] parameters)
        => ExecuteScalar<T>(Connection, sql, parameters);

    private static T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
        => Execute(connection, command => (T)command.ExecuteScalar()!, sql, false, parameters);

    // ReSharper disable once UnusedMember.Global
    public Task<T> ExecuteScalarAsync<T>(string sql, params object[] parameters)
        => ExecuteScalarAsync<T>(Connection, sql, parameters);

    private static Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql, object[]? parameters = null)
        => ExecuteAsync(connection, async command => (T)(await command.ExecuteScalarAsync())!, sql, false, parameters);

    // ReSharper disable once UnusedMethodReturnValue.Global
    public int ExecuteNonQuery(string sql, params object[] parameters)
        => ExecuteNonQuery(Connection, sql, parameters);

    private static int ExecuteNonQuery(DbConnection connection, string sql, object[]? parameters = null)
        => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

    // ReSharper disable once UnusedMember.Global
    public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
        => ExecuteNonQueryAsync(Connection, sql, parameters);

    private static Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql, object[]? parameters = null)
        => ExecuteAsync(connection, command => command.ExecuteNonQueryAsync(), sql, false, parameters);

    // ReSharper disable once UnusedMember.Global
    public IEnumerable<T> Query<T>(string sql, params object[] parameters)
        => Query<T>(Connection, sql, parameters);

    private static IEnumerable<T> Query<T>(DbConnection connection, string sql, object[]? parameters = null)
        => Execute(
            connection, command =>
            {
                using var dataReader = command.ExecuteReader();

                var results = Enumerable.Empty<T>();
                while (dataReader.Read())
                {
                    results = results.Concat([dataReader.GetFieldValue<T>(0)]);
                }

                return results;
            }, sql, false, parameters);

    // ReSharper disable once UnusedMember.Global
    public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
        => QueryAsync<T>(Connection, sql, parameters);

    private static Task<IEnumerable<T>> QueryAsync<T>(DbConnection connection, string sql, object[]? parameters = null)
        => ExecuteAsync(
            connection, async command =>
            {
                await using var dataReader = await command.ExecuteReaderAsync();

                var results = Enumerable.Empty<T>();
                while (await dataReader.ReadAsync())
                {
                    results = results.Concat([await dataReader.GetFieldValueAsync<T>(0)]);
                }

                return results;
            }, sql, false, parameters);

    private static T Execute<T>(
        DbConnection connection,
        Func<DbCommand, T> execute,
        string sql,
        bool useTransaction = false,
        object[]? parameters = null)
        => ExecuteCommand(connection, execute, sql, useTransaction, parameters);

    private static T ExecuteCommand<T>(
        DbConnection connection,
        Func<DbCommand, T> execute,
        string sql,
        bool useTransaction,
        object[]? parameters)
    {
        if (connection.State != ConnectionState.Closed)
        {
            connection.Close();
        }

        connection.Open();
        try
        {
            using var transaction = useTransaction ? connection.BeginTransaction() : null;

            T result;
            using (var command = CreateCommand(connection, sql, parameters))
            {
                command.Transaction = transaction;
                result = execute(command);
            }

            transaction?.Commit();

            return result;
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }

    private static Task<T> ExecuteAsync<T>(
        DbConnection connection,
        Func<DbCommand, Task<T>> executeAsync,
        string sql,
        bool useTransaction = false,
        IReadOnlyList<object>? parameters = null)
        => ExecuteCommandAsync(connection, executeAsync, sql, useTransaction, parameters);

    private static async Task<T> ExecuteCommandAsync<T>(
        DbConnection connection,
        Func<DbCommand, Task<T>> executeAsync,
        string sql,
        bool useTransaction,
        IReadOnlyList<object>? parameters)
    {
        if (connection.State != ConnectionState.Closed)
        {
            await connection.CloseAsync();
        }

        await connection.OpenAsync();
        try
        {
            await using var transaction = useTransaction ? await connection.BeginTransactionAsync() : null;

            T result;
            await using (var command = CreateCommand(connection, sql, parameters))
            {
                result = await executeAsync(command);
            }

            if (transaction is not null)
            {
                await transaction.CommitAsync();
            }

            return result;
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static DbCommand CreateCommand(
        DbConnection connection,
        string commandText,
        IReadOnlyList<object>? parameters = null)
    {
        var command = (GaussDBCommand)connection.CreateCommand();

        command.CommandText = commandText;
        command.CommandTimeout = CommandTimeout;

        if (parameters is not null)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                command.Parameters.AddWithValue("p" + i, parameters[i]);
            }
        }

        return command;
    }

    public static string CreateConnectionString(
        string name,
        string? options = null,
        bool? enableExtensionSessionParameter = null)
    {
        var builder = new GaussDBConnectionStringBuilder(TestEnvironment.DefaultConnection) { Database = name };
        var connectionStringOptions = CreateConnectionStringOptions(
            options,
            enableExtensionSessionParameter ?? TestEnvironment.EnableExtensionSessionParameter);

        if (connectionStringOptions is null)
        {
            builder.Remove(nameof(GaussDBConnectionStringBuilder.Options));
        }
        else
        {
            builder.Options = connectionStringOptions;
        }

        return builder.ConnectionString;
    }

    private static string? CreateConnectionStringOptions(string? options, bool enableExtensionSessionParameter)
        => enableExtensionSessionParameter
            ? options is null
                ? "-c enable_extension=on"
                : $"-c enable_extension=on {options}"
            : options;

    private static string CreateAdminConnectionString()
        => CreateConnectionString("postgres");

    public override Task CleanAsync(DbContext context)
    {
        context.Database.EnsureClean();
        return Task.CompletedTask;
    }
}
