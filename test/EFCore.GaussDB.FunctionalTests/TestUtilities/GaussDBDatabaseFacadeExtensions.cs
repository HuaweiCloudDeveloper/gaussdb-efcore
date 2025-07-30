namespace Microsoft.EntityFrameworkCore.TestUtilities;

public static class GaussDBDatabaseFacadeExtensions
{
    public static void EnsureClean(this DatabaseFacade databaseFacade)
        => databaseFacade.CreateExecutionStrategy()
            .Execute(databaseFacade, database => new GaussDBDatabaseCleaner().Clean(database));
}
