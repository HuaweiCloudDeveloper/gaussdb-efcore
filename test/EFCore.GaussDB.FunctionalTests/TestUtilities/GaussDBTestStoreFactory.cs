namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBTestStoreFactory(
        string? scriptPath = null,
        string? additionalSql = null,
        string? connectionStringOptions = null,
        bool useConnectionString = false) : RelationalTestStoreFactory
{
    public static GaussDBTestStoreFactory Instance { get; } = new();

    public override TestStore Create(string storeName)
        => new GaussDBTestStore(storeName, scriptPath, additionalSql, connectionStringOptions, shared: false) { UseConnectionString = useConnectionString };

    public override TestStore GetOrCreate(string storeName)
        => new GaussDBTestStore(storeName, scriptPath, additionalSql, connectionStringOptions, shared: true) { UseConnectionString = useConnectionString };

    public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
        => serviceCollection.AddEntityFrameworkGaussDB();
}
