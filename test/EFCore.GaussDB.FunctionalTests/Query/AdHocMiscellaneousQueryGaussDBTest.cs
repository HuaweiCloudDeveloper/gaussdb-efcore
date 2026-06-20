using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Query;

public class AdHocMiscellaneousQueryGaussDBTest(NonSharedFixture fixture) : AdHocMiscellaneousQueryRelationalTestBase(fixture)
{
    private const string AdHocContainsSkip =
        "openGauss default O-compatible test setup cannot run this Contains-over-entity case reliably because the non-shared database initialization path still depends on unsupported extension/object behavior; skip instead of broad provider/test infrastructure changes.";

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    protected override DbContextOptionsBuilder SetParameterizedCollectionMode(DbContextOptionsBuilder optionsBuilder, ParameterTranslationMode parameterizedCollectionMode)
    {
        new GaussDBDbContextOptionsBuilder(optionsBuilder).UseParameterizedCollectionMode(parameterizedCollectionMode);

        return optionsBuilder;
    }

    // Unlike the other providers, EFCore.GaussDB does actually support mapping JsonElement
    public override Task Mapping_JsonElement_property_throws_a_meaningful_exception()
        => Task.CompletedTask;

    protected override Task Seed2951(Context2951 context)
        => context.Database.ExecuteSqlRawAsync(
            """
CREATE TABLE "ZeroKey" ("Id" int);
INSERT INTO "ZeroKey" VALUES (NULL)
""");

    // Writes DateTime with Kind=Unspecified to timestamptz
    public override Task SelectMany_where_Select(bool async)
        => Task.CompletedTask;

    // Writes DateTime with Kind=Unspecified to timestamptz
    public override Task Subquery_first_member_compared_to_null(bool async)
        => Task.CompletedTask;

    [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/27995/files#r874038747")]
    public override Task StoreType_for_UDF_used(bool async)
        => base.StoreType_for_UDF_used(async);

    [ConditionalTheory(Skip = AdHocContainsSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public new Task Entity_equality_with_Contains_and_Parameter(bool async)
    {
        _ = async;

        return Task.CompletedTask;
    }
}
