using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore;

#nullable disable

public class GaussDBApiConsistencyTest(GaussDBApiConsistencyTest.GaussDBApiConsistencyFixture fixture)
    : ApiConsistencyTestBase<GaussDBApiConsistencyTest.GaussDBApiConsistencyFixture>(fixture)
{
    protected override void AddServices(ServiceCollection serviceCollection)
        => serviceCollection.AddEntityFrameworkGaussDB();

    protected override Assembly TargetAssembly
        => typeof(GaussDBRelationalConnection).Assembly;

    public class GaussDBApiConsistencyFixture : ApiConsistencyFixtureBase
    {
        public override HashSet<Type> FluentApiTypes { get; } =
        [
            typeof(GaussDBDbContextOptionsBuilder),
            typeof(GaussDBDbContextOptionsBuilderExtensions),
            typeof(GaussDBMigrationBuilderExtensions),
            typeof(GaussDBIndexBuilderExtensions),
            typeof(GaussDBModelBuilderExtensions),
            typeof(GaussDBPropertyBuilderExtensions),
            typeof(GaussDBEntityTypeBuilderExtensions),
            typeof(GaussDBServiceCollectionExtensions)
        ];

        public override HashSet<MethodInfo> UnmatchedMetadataMethods { get; } =
        [
            typeof(GaussDBPropertyBuilderExtensions).GetMethod(
                nameof(GaussDBPropertyBuilderExtensions.IsGeneratedTsVectorColumn),
                [typeof(PropertyBuilder), typeof(string), typeof(string[])])
        ];

        public override
            Dictionary<Type,
                (Type ReadonlyExtensions,
                Type MutableExtensions,
                Type ConventionExtensions,
                Type ConventionBuilderExtensions,
                Type RuntimeExtensions)> MetadataExtensionTypes { get; }
            = new()
            {
                {
                    typeof(IReadOnlyModel), (
                        typeof(GaussDBModelExtensions),
                        typeof(GaussDBModelExtensions),
                        typeof(GaussDBModelExtensions),
                        typeof(GaussDBModelBuilderExtensions),
                        null
                    )
                },
                {
                    typeof(IReadOnlyEntityType), (
                        typeof(GaussDBEntityTypeExtensions),
                        typeof(GaussDBEntityTypeExtensions),
                        typeof(GaussDBEntityTypeExtensions),
                        typeof(GaussDBEntityTypeBuilderExtensions),
                        null
                    )
                },
                {
                    typeof(IReadOnlyProperty), (
                        typeof(GaussDBPropertyExtensions),
                        typeof(GaussDBPropertyExtensions),
                        typeof(GaussDBPropertyExtensions),
                        typeof(GaussDBPropertyBuilderExtensions),
                        null
                    )
                },
                {
                    typeof(IReadOnlyIndex), (
                        typeof(GaussDBIndexExtensions),
                        typeof(GaussDBIndexExtensions),
                        typeof(GaussDBIndexExtensions),
                        typeof(GaussDBIndexBuilderExtensions),
                        null
                    )
                }
            };

        public override HashSet<MethodInfo> MetadataMethodExceptions { get; } =
        [
            typeof(GaussDBEntityTypeExtensions).GetRuntimeMethod(
                nameof(GaussDBEntityTypeExtensions.SetStorageParameter),
                [typeof(IConventionEntityType), typeof(string), typeof(object), typeof(bool)])
        ];
    }
}
