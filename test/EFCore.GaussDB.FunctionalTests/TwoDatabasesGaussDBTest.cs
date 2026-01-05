namespace Microsoft.EntityFrameworkCore;

public class TwoDatabasesGaussDBTest(GaussDBFixture fixture) : TwoDatabasesTestBase(fixture), IClassFixture<GaussDBFixture>
{
    protected new GaussDBFixture Fixture
        => (GaussDBFixture)base.Fixture;

    protected override DbContextOptionsBuilder CreateTestOptions(
        DbContextOptionsBuilder optionsBuilder,
        bool withConnectionString = false,
        bool withNullConnectionString = false)
        => withConnectionString
            ? withNullConnectionString
                ? optionsBuilder.UseGaussDB((string?)null)
                : optionsBuilder.UseGaussDB(DummyConnectionString)
            : optionsBuilder.UseGaussDB();

    protected override TwoDatabasesWithDataContext CreateBackingContext(string databaseName)
        => new(Fixture.CreateOptions(GaussDBTestStore.Create(databaseName)));

    protected override string DummyConnectionString { get; } = "Host=localhost;Database=DoesNotExist";
}
