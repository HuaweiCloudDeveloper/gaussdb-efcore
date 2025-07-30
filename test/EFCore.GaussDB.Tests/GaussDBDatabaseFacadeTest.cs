namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBDatabaseFacadeTest
{
    [Fact]
    public void IsGaussDB_when_using_OnConfiguring()
    {
        using var context = new GaussDBOnConfiguringContext();

        Assert.True(context.Database.IsGaussDB());
    }

    [Fact]
    public void IsGaussDB_in_OnModelCreating_when_using_OnConfiguring()
    {
        using var context = new GaussDBOnModelContext();
        var _ = context.Model; // Trigger context initialization

        Assert.True(context.IsGaussDBSet);
    }

    [Fact]
    public void IsGaussDB_in_constructor_when_using_OnConfiguring()
    {
        using var context = new GaussDBConstructorContext();
        var _ = context.Model; // Trigger context initialization

        Assert.True(context.IsGaussDBSet);
    }

    [Fact]
    public void Cannot_use_IsGaussDB_in_OnConfiguring()
    {
        using var context = new GaussDBUseInOnConfiguringContext();

        Assert.Equal(
            CoreStrings.RecursiveOnConfiguring,
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    var _ = context.Model; // Trigger context initialization
                }).Message);
    }

    [Fact]
    public void IsGaussDB_when_using_constructor()
    {
        using var context = new ProviderContext(
            new DbContextOptionsBuilder().UseGaussDB("Database=Maltesers").Options);

        Assert.True(context.Database.IsGaussDB());
    }

    [Fact]
    public void IsGaussDB_in_OnModelCreating_when_using_constructor()
    {
        using var context = new ProviderOnModelContext(
            new DbContextOptionsBuilder().UseGaussDB("Database=Maltesers").Options);
        var _ = context.Model; // Trigger context initialization

        Assert.True(context.IsGaussDBSet);
    }

    [Fact]
    public void IsGaussDB_in_constructor_when_using_constructor()
    {
        using var context = new ProviderConstructorContext(
            new DbContextOptionsBuilder().UseGaussDB("Database=Maltesers").Options);
        var _ = context.Model; // Trigger context initialization

        Assert.True(context.IsGaussDBSet);
    }

    [Fact]
    public void Cannot_use_IsGaussDB_in_OnConfiguring_with_constructor()
    {
        using var context = new ProviderUseInOnConfiguringContext(
            new DbContextOptionsBuilder().UseGaussDB("Database=Maltesers").Options);

        Assert.Equal(
            CoreStrings.RecursiveOnConfiguring,
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    var _ = context.Model; // Trigger context initialization
                }).Message);
    }

    /*
    [Fact]
    public void Not_IsGaussDB_when_using_different_provider()
    {
        using var context = new ProviderContext(
            new DbContextOptionsBuilder().UseInMemoryDatabase("Maltesers").Options);

        Assert.False(context.Database.IsGaussDB());
    }*/

    private class ProviderContext : DbContext
    {
        protected ProviderContext()
        {
        }

        public ProviderContext(DbContextOptions options)
            : base(options)
        {
        }

        public bool? IsGaussDBSet { get; protected set; }
    }

    private class GaussDBOnConfiguringContext : ProviderContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseGaussDB("Database=Maltesers");
    }

    private class GaussDBOnModelContext : GaussDBOnConfiguringContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => IsGaussDBSet = Database.IsGaussDB();
    }

    private class GaussDBConstructorContext : GaussDBOnConfiguringContext
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        public GaussDBConstructorContext()
        {
            IsGaussDBSet = Database.IsGaussDB();
        }
    }

    private class GaussDBUseInOnConfiguringContext : GaussDBOnConfiguringContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            IsGaussDBSet = Database.IsGaussDB();
        }
    }

    private class ProviderOnModelContext(DbContextOptions options) : ProviderContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => IsGaussDBSet = Database.IsGaussDB();
    }

    private class ProviderConstructorContext : ProviderContext
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        public ProviderConstructorContext(DbContextOptions options)
            : base(options)
        {
            IsGaussDBSet = Database.IsGaussDB();
        }
    }

    private class ProviderUseInOnConfiguringContext(DbContextOptions options) : ProviderContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => IsGaussDBSet = Database.IsGaussDB();
    }
}
