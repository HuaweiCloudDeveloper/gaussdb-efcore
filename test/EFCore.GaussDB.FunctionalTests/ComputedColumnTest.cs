namespace Microsoft.EntityFrameworkCore;

[MinimumPostgresVersion(12, 0)]
public class ComputedColumnTest : IAsyncLifetime
{
    [ConditionalFact]
    public void Can_use_computed_columns()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkGaussDB()
            .BuildServiceProvider();

        using var context = new Context(serviceProvider, TestStore.Name);
        context.Database.EnsureCreatedResiliently();

        var entity = context.Add(
            new Entity
            {
                P1 = 20,
                P2 = 30,
                P3 = 80
            }).Entity;

        context.SaveChanges();

        Assert.Equal(50, entity.P4);
        Assert.Equal(100, entity.P5);
    }

    [ConditionalFact]
    public void Can_use_computed_columns_with_null_values()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkGaussDB()
            .BuildServiceProvider();

        using var context = new Context(serviceProvider, TestStore.Name);
        context.Database.EnsureCreatedResiliently();

        var entity = context.Add(new Entity { P1 = 20, P2 = 30 }).Entity;

        context.SaveChanges();

        Assert.Equal(50, entity.P4);
        Assert.Null(entity.P5);
    }

    private class Context(IServiceProvider serviceProvider, string databaseName) : DbContext
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly string _databaseName = databaseName;

        public DbSet<Entity> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseGaussDB(GaussDBTestStore.CreateConnectionString(_databaseName), b => b.ApplyConfiguration())
                .UseInternalServiceProvider(_serviceProvider);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .Property(e => e.P4)
                .HasComputedColumnSql("""
                    "P1" + "P2"
                    """, stored: true);

            modelBuilder.Entity<Entity>()
                .Property(e => e.P5)
                .HasComputedColumnSql("""
                    "P1" + "P3"
                    """, stored: true);
        }
    }

    private class Entity
    {
        public int Id { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int? P3 { get; set; }
        public int P4 { get; set; }
        public int? P5 { get; set; }
    }

    [Flags]
    public enum FlagEnum
    {
        None = 0x0,
        AValue = 0x1,
        BValue = 0x2
    }

    public class EnumItem
    {
        public int EnumItemId { get; set; }
        public FlagEnum FlagEnum { get; set; }
        public FlagEnum? OptionalFlagEnum { get; set; }
        public FlagEnum? CalculatedFlagEnum { get; set; }
    }

    private class NullableContext(IServiceProvider serviceProvider, string databaseName) : DbContext
    {
        public DbSet<EnumItem> EnumItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseGaussDB(GaussDBTestStore.CreateConnectionString(databaseName), b => b.ApplyConfiguration())
                .UseInternalServiceProvider(serviceProvider);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<EnumItem>()
                .Property(entity => entity.CalculatedFlagEnum)
                .HasComputedColumnSql("""
                    "FlagEnum" | "OptionalFlagEnum"
                    """, stored: true);
    }

    [ConditionalFact]
    public void Can_use_computed_columns_with_nullable_enum()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkGaussDB()
            .BuildServiceProvider();

        using var context = new NullableContext(serviceProvider, TestStore.Name);
        context.Database.EnsureCreatedResiliently();

        var entity = context.EnumItems.Add(new EnumItem { FlagEnum = FlagEnum.AValue, OptionalFlagEnum = FlagEnum.BValue }).Entity;
        context.SaveChanges();

        Assert.Equal(FlagEnum.AValue | FlagEnum.BValue, entity.CalculatedFlagEnum);
    }

    protected GaussDBTestStore TestStore { get; private set; } = null!;

    public async Task InitializeAsync()
        => TestStore = await GaussDBTestStore.CreateInitializedAsync("ComputedColumnTest");

    public async Task DisposeAsync()
        => await TestStore.DisposeAsync();
}
