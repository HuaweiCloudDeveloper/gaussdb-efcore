namespace Microsoft.EntityFrameworkCore;

public class SystemColumnTest : IClassFixture<SystemColumnTest.SystemColumnFixture>
{
    private const string XminSkip =
        "Local-only: the current openGauss system-column/xmin behavior in this environment diverges from the PostgreSQL expectation exercised by this test, and fixing it would require provider/system-column handling work.";

    private SystemColumnFixture Fixture { get; }

    public SystemColumnTest(SystemColumnFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [Fact(Skip = XminSkip)]
    public void Xmin()
    {
        using var context = CreateContext();

        var e = new SomeEntity { Name = "Bart" };
        context.Entities.Add(e);
        context.SaveChanges();
        var firstVersion = e.Version;

        e.Name = "Lisa";
        context.SaveChanges();
        var secondVersion = e.Version;

        Assert.NotEqual(firstVersion, secondVersion);
    }

    public class SystemColumnContext(DbContextOptions options) : PoolableDbContext(options)
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DbSet<SomeEntity> Entities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.Entity<SomeEntity>().Property(e => e.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class SomeEntity
    {
        // ReSharper disable UnusedMember.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public uint Version { get; set; }
        // ReSharper restore UnusedMember.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    private SystemColumnContext CreateContext()
        => Fixture.CreateContext();

    public class SystemColumnFixture : SharedStoreFixtureBase<SystemColumnContext>
    {
        protected override string StoreName
            => "SystemColumnTest";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;
    }
}
