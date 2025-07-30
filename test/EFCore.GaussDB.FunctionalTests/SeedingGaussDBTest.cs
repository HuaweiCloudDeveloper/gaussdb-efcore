namespace Microsoft.EntityFrameworkCore;

public class SeedingGaussDBTest : SeedingTestBase
{
    protected override TestStore TestStore
        => GaussDBTestStore.Create("SeedingTest");

    protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
        => new SeedingGaussDBContext(testId);

    protected class SeedingGaussDBContext(string testId) : SeedingContext(testId)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB(GaussDBTestStore.CreateConnectionString($"Seeds{TestId}"));
    }
}
