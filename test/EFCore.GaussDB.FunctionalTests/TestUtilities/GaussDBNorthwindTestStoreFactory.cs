namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBNorthwindTestStoreFactory : GaussDBTestStoreFactory
{
    public const string Name = "Northwind";
    public static readonly string NorthwindConnectionString = GaussDBTestStore.CreateConnectionString(Name);
    public static new GaussDBNorthwindTestStoreFactory Instance { get; } = new();

    static GaussDBNorthwindTestStoreFactory()
    {
        // TODO: Switch to using GaussDBDataSource
#pragma warning disable CS0618 // Type or member is obsolete
        GaussDBConnection.GlobalTypeMapper.EnableDynamicJson();
        GaussDBConnection.GlobalTypeMapper.EnableRecordsAsTuples();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected GaussDBNorthwindTestStoreFactory()
    {
    }

    public override TestStore GetOrCreate(string storeName)
        => GaussDBTestStore.GetOrCreate(
            Name,
            scriptPath: "Northwind.sql",
            additionalSql: TestEnvironment.PostgresVersion >= new Version(12, 0)
                ? """CREATE COLLATION IF NOT EXISTS "some-case-insensitive-collation" (LOCALE = 'en-u-ks-primary', PROVIDER = icu, DETERMINISTIC = False);"""
                : null);
}
