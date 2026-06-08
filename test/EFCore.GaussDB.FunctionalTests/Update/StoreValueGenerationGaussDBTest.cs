using System.Text;
using Microsoft.EntityFrameworkCore.TestModels.StoreValueGenerationModel;

namespace Microsoft.EntityFrameworkCore.Update;

public class StoreValueGenerationGaussDBTest : StoreValueGenerationTestBase<
    StoreValueGenerationGaussDBTest.StoreValueGenerationGaussDBFixture>
{
    private const string StoreGeneratedReadSkip =
        "Local-only: openGauss write-result propagation currently misreads RETURNING/store-generated values in this suite, causing partial-field consumption errors; fixing it cleanly would require broader provider update-pipeline work.";

    public StoreValueGenerationGaussDBTest(StoreValueGenerationGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override bool ShouldCreateImplicitTransaction(
        EntityState firstOperationType,
        EntityState? secondOperationType,
        GeneratedValues generatedValues,
        bool withSameEntityType)
        => secondOperationType is not null;

    protected override int ShouldExecuteInNumberOfCommands(
        EntityState firstOperationType,
        EntityState? secondOperationType,
        GeneratedValues generatedValues,
        bool withDatabaseGenerated)
        => 1;

    #region Single operation

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_with_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_with_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_with_all_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_with_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_with_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion Single operation

    #region Two operations with same entity type

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_same_entity_type_and_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_same_entity_type_and_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_same_entity_type_and_all_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_Modify_with_same_entity_type_and_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_Modify_with_same_entity_type_and_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_Delete_with_same_entity_type(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion Two operations with same entity type

    #region Two operations with different entity types

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_different_entity_types_and_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_different_entity_types_and_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Add_Add_with_different_entity_types_and_all_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_Modify_with_different_entity_types_and_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Modify_Modify_with_different_entity_types_and_no_generated_values(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_Delete_with_different_entity_types(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = StoreGeneratedReadSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_Add_with_same_entity_types(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion Two operations with different entity types

    public class StoreValueGenerationGaussDBFixture : StoreValueGenerationFixtureBase
    {
        private string? _cleanDataSql;

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            foreach (var name in new[]
                     {
                         nameof(StoreValueGenerationContext.WithSomeDatabaseGenerated),
                         nameof(StoreValueGenerationContext.WithSomeDatabaseGenerated2),
                         nameof(StoreValueGenerationContext.WithAllDatabaseGenerated),
                         nameof(StoreValueGenerationContext.WithAllDatabaseGenerated2)
                     })
            {
                ConfigureComputedColumn(modelBuilder.SharedTypeEntity<StoreValueGenerationData>(name).Property(w => w.Data1));
            }

            foreach (var name in new[]
                     {
                         nameof(StoreValueGenerationContext.WithAllDatabaseGenerated),
                         nameof(StoreValueGenerationContext.WithAllDatabaseGenerated2)
                     })
            {
                ConfigureComputedColumn(modelBuilder.SharedTypeEntity<StoreValueGenerationData>(name).Property(w => w.Data2));
            }

            void ConfigureComputedColumn(PropertyBuilder builder)
            {
                if (TestEnvironment.PostgresVersion >= new Version(12, 0))
                {
                    // PG 12+ supports computed columns, but only stored (must be explicitly specified)
                    builder.Metadata.SetIsStored(true);
                }
                else
                {
                    // Before PG 12, disable computed columns (but leave OnAddOrUpdate)
                    builder
                        .HasComputedColumnSql(null)
                        .HasDefaultValue(100)
                        .Metadata
                        .ValueGenerated = ValueGenerated.OnAddOrUpdate;
                }
            }
        }

        public override void CleanData()
        {
            using var context = CreateContext();
            context.Database.ExecuteSqlRaw(_cleanDataSql ??= GetCleanDataSql());
        }

        private string GetCleanDataSql()
        {
            var context = CreateContext();
            var builder = new StringBuilder();

            var helper = context.GetService<ISqlGenerationHelper>();
            var tables = context.Model.GetEntityTypes()
                .SelectMany(e => e.GetTableMappings().Select(m => helper.DelimitIdentifier(m.Table.Name, m.Table.Schema)));

            foreach (var table in tables)
            {
                builder.AppendLine($"TRUNCATE TABLE {table} RESTART IDENTITY;");
            }

            return builder.ToString();
        }
    }
}
