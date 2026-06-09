using Microsoft.EntityFrameworkCore.TestModels.ManyToManyModel;

namespace Microsoft.EntityFrameworkCore;

public class ManyToManyTrackingGaussDBTest(ManyToManyTrackingGaussDBTest.ManyToManyTrackingGaussDBFixture fixture)
    : ManyToManyTrackingRelationalTestBase<
        ManyToManyTrackingGaussDBTest.ManyToManyTrackingGaussDBFixture>(fixture)
{
    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public override async Task Can_insert_many_to_many_composite_with_navs_unidirectional(bool async)
    {
        List<int>? leftKeys = null;

        await ExecuteWithStrategyInTransactionAsync(
            async context =>
            {
                var leftEntities = new[]
                {
                    context.UnidirectionalEntityCompositeKeys.CreateInstance(
                        (e, _) =>
                        {
                            e.Key1 = !Fixture.UseGeneratedKeys ? 7711 : 0;
                            e.Key2 = "7711";
                            e.Key3 = new DateTime(7711, 1, 1);
                        }),
                    context.UnidirectionalEntityCompositeKeys.CreateInstance(
                        (e, _) =>
                        {
                            e.Key1 = !Fixture.UseGeneratedKeys ? 7712 : 0;
                            e.Key2 = "7712";
                            e.Key3 = new DateTime(7712, 1, 1);
                        }),
                    context.UnidirectionalEntityCompositeKeys.CreateInstance(
                        (e, _) =>
                        {
                            e.Key1 = !Fixture.UseGeneratedKeys ? 7713 : 0;
                            e.Key2 = "7713";
                            e.Key3 = new DateTime(7713, 1, 1);
                        })
                };

                var rightEntities = new[]
                {
                    context.Set<UnidirectionalEntityLeaf>().CreateInstance((e, _) => e.Id = !Fixture.UseGeneratedKeys ? 7721 : 0),
                    context.Set<UnidirectionalEntityLeaf>().CreateInstance((e, _) => e.Id = !Fixture.UseGeneratedKeys ? 7722 : 0),
                    context.Set<UnidirectionalEntityLeaf>().CreateInstance((e, _) => e.Id = !Fixture.UseGeneratedKeys ? 7723 : 0)
                };

                rightEntities[0].CompositeKeySkipFull = [];
                rightEntities[1].CompositeKeySkipFull = [];
                rightEntities[2].CompositeKeySkipFull = [];
                rightEntities[0].CompositeKeySkipFull.Add(leftEntities[0]);
                rightEntities[1].CompositeKeySkipFull.Add(leftEntities[0]);
                rightEntities[2].CompositeKeySkipFull.Add(leftEntities[0]);
                rightEntities[0].CompositeKeySkipFull.Add(leftEntities[1]);
                rightEntities[0].CompositeKeySkipFull.Add(leftEntities[2]);

                if (async)
                {
                    await context.AddRangeAsync(leftEntities[0], leftEntities[1], leftEntities[2]);
                    await context.AddRangeAsync(rightEntities[0], rightEntities[1], rightEntities[2]);
                }
                else
                {
                    context.AddRange(leftEntities[0], leftEntities[1], leftEntities[2]);
                    context.AddRange(rightEntities[0], rightEntities[1], rightEntities[2]);
                }

                ValidateFixup(context, leftEntities, rightEntities);
                await context.SaveChangesAsync();
                ValidateFixup(context, leftEntities, rightEntities);
                leftKeys = leftEntities.Select(e => e.Key1).ToList();
            },
            async context =>
            {
                var source = context.Set<UnidirectionalEntityCompositeKey>()
                    .Where(e => leftKeys!.Contains(e.Key1));

                context.Set<UnidirectionalJoinCompositeKeyToLeaf>()
                    .Where(e => leftKeys!.Contains(e.CompositeId1))
                    .Include(e => e.Leaf)
                    .Load();

                var list = async ? await source.ToListAsync() : source.ToList();

                Assert.Equal(3, list.Count);

                var leftEntities = context.ChangeTracker.Entries<UnidirectionalEntityCompositeKey>()
                    .Select(e => e.Entity)
                    .OrderBy(e => e.Key2)
                    .ToList();
                var rightEntities = context.ChangeTracker.Entries<UnidirectionalEntityLeaf>()
                    .Select(e => e.Entity)
                    .OrderBy(e => e.Id)
                    .ToList();

                ValidateFixup(context, leftEntities, rightEntities);
            });

        static void ValidateFixup(
            DbContext context,
            IList<UnidirectionalEntityCompositeKey> leftEntities,
            IList<UnidirectionalEntityLeaf> rightEntities)
        {
            Assert.Equal(11, context.ChangeTracker.Entries().Count());
            Assert.Equal(3, context.ChangeTracker.Entries<UnidirectionalEntityCompositeKey>().Count());
            Assert.Equal(3, context.ChangeTracker.Entries<UnidirectionalEntityLeaf>().Count());
            Assert.Equal(5, context.ChangeTracker.Entries<UnidirectionalJoinCompositeKeyToLeaf>().Count());
            Assert.Equal(3, rightEntities[0].CompositeKeySkipFull.Count);
            Assert.Single(rightEntities[1].CompositeKeySkipFull);
            Assert.Single(rightEntities[2].CompositeKeySkipFull);

            var joins = context.ChangeTracker.Entries<UnidirectionalJoinCompositeKeyToLeaf>().Select(e => e.Entity).ToList();
            foreach (var join in joins)
            {
                Assert.Equal(join.Composite.Key1, join.CompositeId1);
                Assert.Equal(join.Composite.Key2, join.CompositeId2);
                Assert.Equal(join.Composite.Key3, join.CompositeId3);
                Assert.Equal(join.Leaf.Id, join.LeafId);
                Assert.Contains(join, join.Composite.JoinLeafFull);
                Assert.Contains(join, join.Leaf.JoinCompositeKeyFull);
            }
        }
    }

    public class ManyToManyTrackingGaussDBFixture : ManyToManyTrackingRelationalFixture, ITestSqlLoggerFactory
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            modelBuilder
                .Entity<JoinOneSelfPayload>()
                .Property(e => e.Payload)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            modelBuilder
                .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                .IndexerProperty<string>("Payload")
                .HasDefaultValue("Generated");

            modelBuilder
                .Entity<JoinOneToThreePayloadFull>()
                .Property(e => e.Payload)
                .HasDefaultValue("Generated");

            modelBuilder
                .Entity<UnidirectionalJoinOneSelfPayload>()
                .Property(e => e.Payload)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            modelBuilder
                .SharedTypeEntity<Dictionary<string, object>>("UnidirectionalJoinOneToThreePayloadFullShared")
                .IndexerProperty<string>("Payload")
                .HasDefaultValue("Generated");

            modelBuilder
                .Entity<UnidirectionalJoinOneToThreePayloadFull>()
                .Property(e => e.Payload)
                .HasDefaultValue("Generated");

            // Additional GaussDB-specific config (for timestamp without time zone)
            modelBuilder
                .Entity<UnidirectionalEntityCompositeKey>()
                .Property(e => e.Key3)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<UnidirectionalJoinCompositeKeyToLeaf>()
                .Property(e => e.CompositeId3)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<EntityCompositeKey>()
                .Property(e => e.Key3)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<JoinCompositeKeyToLeaf>()
                .Property(e => e.CompositeId3)
                .HasColumnType("timestamp without time zone");
        }
    }
}
