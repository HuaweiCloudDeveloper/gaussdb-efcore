using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Diagnostics.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Scaffolding.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBDatabaseCleaner : RelationalDatabaseCleaner
{
    private const string InternalSchemas = "'pg_catalog', 'information_schema', 'sys', 'db4ai', 'dbe_perf', 'dbe_pldeveloper', 'dbe_profiler'";

    private readonly GaussDBSqlGenerationHelper _sqlGenerationHelper = new(new RelationalSqlGenerationHelperDependencies());

    protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
        => new GaussDBDatabaseModelFactory(
            new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                loggerFactory,
                new LoggingOptions(),
                new DiagnosticListener("Fake"),
                new GaussDBLoggingDefinitions(),
                new NullDbContextLogger()));

    protected override bool AcceptIndex(DatabaseIndex index)
        => false;

    public override void Clean(DatabaseFacade facade)
    {
        var creator = facade.GetService<IRelationalDatabaseCreator>();
        var connection = facade.GetService<IRelationalConnection>();
        if (creator.Exists())
        {
            connection.Open();
            try
            {
                var conn = (GaussDBConnection)connection.DbConnection;
                DropTypes(conn);
                DropFunctions(conn);
                DropCollations(conn);
            }
            finally
            {
                connection.Close();
            }
        }

        base.Clean(facade);
    }

    /// <summary>
    ///     Drop user-defined ranges and enums, cascading to all tables which depend on them
    /// </summary>
    private void DropTypes(GaussDBConnection conn)
    {
        var getUserDefinedRangesEnums = $"""
SELECT ns.nspname, typname
FROM pg_type
JOIN pg_namespace AS ns ON ns.oid = pg_type.typnamespace
WHERE typtype IN ('r', 'e') AND nspname NOT IN ({InternalSchemas})
""";

        (string Schema, string Name)[] userDefinedTypes;
        using (var cmd = new GaussDBCommand(getUserDefinedRangesEnums, conn))
        {
            using var reader = cmd.ExecuteReader();
            userDefinedTypes = reader.Cast<DbDataRecord>().Select(r => (r.GetString(0), r.GetString(1))).ToArray();
        }

        if (userDefinedTypes.Any())
        {
            var dropTypes = string.Concat(userDefinedTypes.Select(t => $"""DROP TYPE "{t.Schema}"."{t.Name}" CASCADE;"""));
            using var cmd = new GaussDBCommand(dropTypes, conn);
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    ///     Drop all user-defined functions and procedures
    /// </summary>
    private void DropFunctions(GaussDBConnection conn)
    {
        var getUserDefinedFunctions = $"""
SELECT 'DROP FUNCTION "' || nspname || '"."' || proname || '"(' || oidvectortypes(proargtypes) || ');' FROM pg_proc
JOIN pg_namespace AS ns ON ns.oid = pg_proc.pronamespace
WHERE
        nspname NOT IN ({InternalSchemas}) AND
    NOT EXISTS (
            SELECT * FROM pg_depend AS dep
            WHERE dep.classid = (SELECT oid FROM pg_class WHERE relname = 'pg_proc') AND
                    dep.objid = pg_proc.oid AND
                    deptype = 'e');
""";

        string[] dropSql;
        using (var cmd = new GaussDBCommand(getUserDefinedFunctions, conn))
        {
            using var reader = cmd.ExecuteReader();
            dropSql = reader.Cast<DbDataRecord>().Select(r => r.GetString(0)).ToArray();
        }

        foreach (var sql in dropSql)
        {
            try
            {
                using var cmd = new GaussDBCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (PostgresException e) when (e.SqlState is "42P13" or "42501" or "42883")
            {
                // openGauss may expose builtin or protected functions in user-visible schemas; ignore cleanup noise.
            }
        }
    }

    private void DropCollations(GaussDBConnection conn)
    {
        if (conn.PostgreSqlVersion < new Version(9, 1))
        {
            return;
        }

        var getUserCollations =
            $"""
SELECT nspname, collname
FROM pg_collation coll
    JOIN pg_namespace ns ON ns.oid=coll.collnamespace
    JOIN pg_authid auth ON auth.oid = coll.collowner WHERE nspname NOT IN ({InternalSchemas});
""";

        (string Schema, string Name)[] userDefinedTypes;
        using (var cmd = new GaussDBCommand(getUserCollations, conn))
        {
            using var reader = cmd.ExecuteReader();
            userDefinedTypes = reader.Cast<DbDataRecord>().Select(r => (r.GetString(0), r.GetString(1))).ToArray();
        }

        if (userDefinedTypes.Any())
        {
            var dropTypes = string.Concat(userDefinedTypes.Select(t => $"""DROP COLLATION "{t.Schema}"."{t.Name}" CASCADE;"""));
            using var cmd = new GaussDBCommand(dropTypes, conn);
            cmd.ExecuteNonQuery();
        }
    }

    protected override string BuildCustomSql(DatabaseModel databaseModel)
        // The test environment reuses shared extensions managed outside the tests; don't drop them during cleanup.
        => "";

    protected override string BuildCustomEndingSql(DatabaseModel databaseModel)
        => databaseModel.GetPostgresEnums()
            .Select(e => _sqlGenerationHelper.DelimitIdentifier(e.Name, e.Schema))
            .Aggregate(
                new StringBuilder(),
                (builder, s) => builder.Append("DROP TYPE ").Append(s).Append(" CASCADE;"),
                builder => builder.ToString());
}
