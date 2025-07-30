using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

public class MigrationsInfrastructureGaussDBTest(MigrationsInfrastructureGaussDBTest.MigrationsInfrastructureGaussDBFixture fixture)
    : MigrationsInfrastructureTestBase<MigrationsInfrastructureGaussDBTest.MigrationsInfrastructureGaussDBFixture>(fixture)
{
    public override void Can_get_active_provider()
    {
        base.Can_get_active_provider();

        Assert.Equal("HuaweiCloud.EntityFrameworkCore.GaussDB", ActiveProvider);
    }

    // See #3407
    public override void Can_apply_two_migrations_in_transaction()
        => Assert.ThrowsAny<Exception>(() => base.Can_apply_two_migrations_in_transaction());

    // See #3407
    public override Task Can_apply_two_migrations_in_transaction_async()
        => Assert.ThrowsAnyAsync<Exception>(() => base.Can_apply_two_migrations_in_transaction_async());

    // This tests uses Fixture.CreateEmptyContext(), which does not go through MigrationsInfrastructureGaussDBFixture.CreateContext()
    // and therefore does not set the PostgresVersion in the context options. As a result, we try to drop the database with
    // WITH (FORCE), which is only supported starting with PG 13.
    [MinimumPostgresVersion(13, 0)]
    public override Task Can_generate_no_migration_script()
        => base.Can_generate_no_migration_script();

    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/33056")]
    public override void Can_apply_all_migrations()
        => base.Can_apply_all_migrations();

    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/33056")]
    public override void Can_apply_range_of_migrations()
        => base.Can_apply_range_of_migrations();

    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/33056")]
    public override void Can_revert_all_migrations()
        => base.Can_revert_all_migrations();

    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/33056")]
    public override void Can_revert_one_migrations()
        => base.Can_revert_one_migrations();

    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/33056")]
    public override Task Can_apply_all_migrations_async()
        => base.Can_apply_all_migrations_async();

    [ConditionalFact]
    public async Task Empty_Migration_Creates_Database()
    {
        await using var context = new BloggingContext(
            Fixture.TestStore.AddProviderOptions(
                    new DbContextOptionsBuilder().EnableServiceProviderCaching(false))
                .ConfigureWarnings(e => e.Log(RelationalEventId.PendingModelChangesWarning)).Options);

        var creator = (GaussDBDatabaseCreator)context.GetService<IRelationalDatabaseCreator>();
        creator.RetryTimeout = TimeSpan.FromMinutes(10);

        await context.Database.MigrateAsync();

        Assert.True(creator.Exists());
    }

    private class BloggingContext(DbContextOptions options) : DbContext(options)
    {
        // ReSharper disable once UnusedMember.Local
        public DbSet<Blog> Blogs { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Local
        public class Blog
        {
            // ReSharper disable UnusedMember.Local
            public int Id { get; set; }

            public string Name { get; set; }
            // ReSharper restore UnusedMember.Local
        }
    }

    [DbContext(typeof(BloggingContext))]
    [Migration("00000000000000_Empty")]
    public class EmptyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }
    }

    public override void Can_diff_against_2_2_model()
    {
        using var context = new Migrations.BloggingContext();
        DiffSnapshot(new BloggingContextModelSnapshot22(), context);
    }

    public class BloggingContextModelSnapshot22 : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation(
                    "GaussDB:ValueGenerationStrategy",
                    GaussDBValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity(
                "ModelSnapshot22.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(
                            "GaussDB:ValueGenerationStrategy",
                            GaussDBValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity(
                "ModelSnapshot22.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(
                            "GaussDB:ValueGenerationStrategy",
                            GaussDBValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("BlogId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("EditDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("BlogId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity(
                "ModelSnapshot22.Post", b =>
                {
                    b.HasOne("ModelSnapshot22.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId");
                });
#pragma warning restore 612, 618
        }
    }

    public override void Can_diff_against_3_0_ASP_NET_Identity_model()
    {
        // TODO: Implement
    }

    public override void Can_diff_against_2_2_ASP_NET_Identity_model()
    {
        // TODO: Implement
    }

    public override void Can_diff_against_2_1_ASP_NET_Identity_model()
    {
        // TODO: Implement
    }

    protected override Task ExecuteSqlAsync(string value)
        => ((GaussDBTestStore)Fixture.TestStore).ExecuteNonQueryAsync(value);

    public class MigrationsInfrastructureGaussDBFixture : MigrationsInfrastructureFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public override MigrationsContext CreateContext()
        {
            var options = AddOptions(
                    TestStore.AddProviderOptions(new DbContextOptionsBuilder())
                        .UseGaussDB(
                            TestStore.ConnectionString, b => b.ApplyConfiguration()
                                .SetPostgresVersion(TestEnvironment.PostgresVersion)))
                .UseInternalServiceProvider(ServiceProvider)
                .Options;
            return new MigrationsContext(options);
        }
    }
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Post> Posts { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime EditDate { get; set; }

    public Blog Blog { get; set; }
}

public class BloggingContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB(TestEnvironment.DefaultConnection);

    public DbSet<Blog> Blogs { get; set; }
}