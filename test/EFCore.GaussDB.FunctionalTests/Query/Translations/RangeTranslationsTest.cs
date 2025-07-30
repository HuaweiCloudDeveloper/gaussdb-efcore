using System.ComponentModel.DataAnnotations.Schema;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

// ReSharper disable InconsistentNaming

namespace Microsoft.EntityFrameworkCore.Query.Translations;

// Note: timestamp range tests are in TimestampQueryTest
public class RangeTranslationsTest : IClassFixture<RangeTranslationsTest.RangeQueryGaussDBFixture>
{
    private RangeQueryGaussDBFixture Fixture { get; }

    // ReSharper disable once UnusedParameter.Local
    public RangeTranslationsTest(RangeQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    #region Operators

    [ConditionalFact]
    public void Contains_value()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.Contains(3));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" @> 3
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Contains_range()
    {
        using var context = CreateContext();

        var range = new GaussDBRange<int>(8, 13);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Contains(range));
        Assert.Equal(2, result.Id);

        AssertSql(
            """
@range='[8,13]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" @> @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void ContainedBy()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(8, 13);
        var result = context.RangeTestEntities.Single(x => range.ContainedBy(x.IntRange));
        Assert.Equal(2, result.Id);

        AssertSql(
            """
@range='[8,13]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE @range <@ r."IntRange"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Equals_operator()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(1, 10);
        var result = context.RangeTestEntities.Single(x => x.IntRange == range);
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[1,10]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" = @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Equals_method()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(1, 10);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Equals(range));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[1,10]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" = @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Overlaps_range()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(-5, 4);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Overlaps(range));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[-5,4]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" && @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void IsStrictlyLeftOf_range()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(11, 15);
        var result = context.RangeTestEntities.Single(x => x.IntRange.IsStrictlyLeftOf(range));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[11,15]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" << @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void IsStrictlyRightOf_range()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(0, 4);
        var result = context.RangeTestEntities.Single(x => x.IntRange.IsStrictlyRightOf(range));
        Assert.Equal(2, result.Id);

        AssertSql(
            """
@range='[0,4]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" >> @range
LIMIT 2
""");
    }

    [ConditionalFact]
    public void DoesNotExtendLeftOf()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(2, 20);
        var result = context.RangeTestEntities.Single(x => range.DoesNotExtendLeftOf(x.IntRange));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[2,20]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE @range &> r."IntRange"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void DoesNotExtendRightOf()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(1, 13);
        var result = context.RangeTestEntities.Single(x => range.DoesNotExtendRightOf(x.IntRange));
        Assert.Equal(2, result.Id);

        AssertSql(
            """
@range='[1,13]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE @range &< r."IntRange"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void IsAdjacentTo()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(2, 4);
        var result = context.RangeTestEntities.Single(x => range.IsAdjacentTo(x.IntRange));
        Assert.Equal(2, result.Id);

        AssertSql(
            """
@range='[2,4]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE @range -|- r."IntRange"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Union()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(-2, 7);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Union(range) == new GaussDBRange<int>(-2, 10));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[-2,7]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" + @range = '[-2,10]'::int4range
LIMIT 2
""");
    }

    [ConditionalFact]
    [MinimumPostgresVersion(14, 0)] // Multiranges were introduced in GaussDB 14
    public void Union_aggregate()
    {
        using var context = CreateContext();

        var union = context.RangeTestEntities
            .Where(x => x.Id == 1 || x.Id == 2)
            .GroupBy(x => true)
            .Select(g => g.Select(x => x.IntRange).RangeAgg())
            .Single();

        Assert.Equal([new(1, true, 16, false)], union);

        AssertSql(
            """
SELECT range_agg(r0."IntRange")
FROM (
    SELECT r."IntRange", TRUE AS "Key"
    FROM "RangeTestEntities" AS r
    WHERE r."Id" IN (1, 2)
) AS r0
GROUP BY r0."Key"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Intersect()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(-2, 3);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Intersect(range) == new GaussDBRange<int>(1, 3));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[-2,3]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" * @range = '[1,3]'::int4range
LIMIT 2
""");
    }

    [ConditionalFact]
    [MinimumPostgresVersion(14, 0)] // range_intersect_agg was introduced in GaussDB 14
    public void Intersect_aggregate()
    {
        using var context = CreateContext();

        var intersection = context.RangeTestEntities
            .Where(x => x.Id == 1 || x.Id == 2)
            .GroupBy(x => true)
            .Select(g => g.Select(x => x.IntRange).RangeIntersectAgg())
            .Single();

        Assert.Equal(new GaussDBRange<int>(5, true, 11, false), intersection);

        AssertSql(
            """
SELECT range_intersect_agg(r0."IntRange")
FROM (
    SELECT r."IntRange", TRUE AS "Key"
    FROM "RangeTestEntities" AS r
    WHERE r."Id" IN (1, 2)
) AS r0
GROUP BY r0."Key"
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Except()
    {
        using var context = CreateContext();
        var range = new GaussDBRange<int>(1, 2);
        var result = context.RangeTestEntities.Single(x => x.IntRange.Except(range) == new GaussDBRange<int>(3, 10));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@range='[1,2]' (DbType = Object)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" - @range = '[3,10]'::int4range
LIMIT 2
""");
    }

    #endregion Operators

    #region Functions

    [ConditionalFact]
    public void LowerBound()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.LowerBound == 1);
        Assert.Equal(1, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE lower(r."IntRange") = 1
LIMIT 2
""");
    }

    [ConditionalFact]
    public void UpperBound()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.UpperBound == 16); // PG normalizes to exclusive
        Assert.Equal(2, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE upper(r."IntRange") = 16
LIMIT 2
""");
    }

    [ConditionalFact]
    public void IsEmpty()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.Intersect(new GaussDBRange<int>(1, 2)).IsEmpty);
        Assert.Equal(2, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE isempty(r."IntRange" * '[1,2]'::int4range)
LIMIT 2
""");
    }

    [ConditionalFact]
    public void LowerBoundIsInclusive()
    {
        using var context = CreateContext();
        var count = context.RangeTestEntities.Count(x => !x.IntRange.LowerBoundIsInclusive);
        Assert.Equal(0, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "RangeTestEntities" AS r
WHERE NOT (lower_inc(r."IntRange"))
""");
    }

    [ConditionalFact]
    public void UpperBoundIsInclusive()
    {
        using var context = CreateContext();
        var count = context.RangeTestEntities.Count(x => x.IntRange.UpperBoundIsInclusive);
        Assert.Equal(0, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "RangeTestEntities" AS r
WHERE upper_inc(r."IntRange")
""");
    }

    [ConditionalFact]
    public void LowerBoundInfinite()
    {
        using var context = CreateContext();
        var count = context.RangeTestEntities.Count(x => x.IntRange.LowerBoundInfinite);
        Assert.Equal(0, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "RangeTestEntities" AS r
WHERE lower_inf(r."IntRange")
""");
    }

    [ConditionalFact]
    public void UpperBoundInfinite()
    {
        using var context = CreateContext();
        var count = context.RangeTestEntities.Count(x => x.IntRange.UpperBoundInfinite);
        Assert.Equal(0, count);

        AssertSql(
            """
SELECT count(*)::int
FROM "RangeTestEntities" AS r
WHERE upper_inf(r."IntRange")
""");
    }

    [ConditionalFact]
    public void Merge()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.Merge(new GaussDBRange<int>(12, 13)) == new GaussDBRange<int>(1, 13));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE range_merge(r."IntRange", '[12,13]'::int4range) = '[1,13]'::int4range
LIMIT 2
""");
    }

    #endregion Functions

    #region Built-in ranges

    [ConditionalFact]
    public void IntRange()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.IntRange.Contains(3));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."IntRange" @> 3
LIMIT 2
""");
    }

    [ConditionalFact]
    public void LongRange()
    {
        using var context = CreateContext();
        var value = 3;
        var result = context.RangeTestEntities.Single(x => x.LongRange.Contains(value));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@p='3'

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."LongRange" @> @p
LIMIT 2
""");
    }

    [ConditionalFact]
    public void DecimalRange()
    {
        using var context = CreateContext();
        var value = 3;
        var result = context.RangeTestEntities.Single(x => x.DecimalRange.Contains(value));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@p='3'

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."DecimalRange" @> @p
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Daterange_DateOnly()
    {
        using var context = CreateContext();
        var value = new DateOnly(2020, 1, 3);
        var result = context.RangeTestEntities.Single(x => x.DateOnlyDateRange.Contains(value));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@value='01/03/2020' (DbType = Date)

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."DateOnlyDateRange" @> @value
LIMIT 2
""");
    }

    [ConditionalFact]
    public void Daterange_DateTime()
    {
        using var context = CreateContext();
        var value = new DateTime(2020, 1, 3);
        var result = context.RangeTestEntities.Single(x => x.DateTimeDateRange.Contains(value));
        Assert.Equal(1, result.Id);

        AssertSql(
            """
@value='2020-01-03T00:00:00.0000000'

SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE r."DateTimeDateRange" @> @value
LIMIT 2
""");
    }

    #endregion Built-in ranges

    #region User-defined ranges

    [ConditionalFact]
    public void User_defined()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.UserDefinedRange.UpperBound > 12.0);
        Assert.Equal(2, result.Id);
        Assert.Equal(5.0, result.UserDefinedRange.LowerBound);
        Assert.Equal(15.0, result.UserDefinedRange.UpperBound);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE upper(r."UserDefinedRange") > 12.0
LIMIT 2
""");
    }

    [ConditionalFact]
    public void User_defined_and_schema_qualified()
    {
        using var context = CreateContext();
        var result = context.RangeTestEntities.Single(x => x.UserDefinedRangeWithSchema.UpperBound > 12.0);
        Assert.Equal(2, result.Id);
        Assert.Equal(5.0, result.UserDefinedRangeWithSchema.LowerBound);
        Assert.Equal(15.0, result.UserDefinedRangeWithSchema.UpperBound);

        AssertSql(
            """
SELECT r."Id", r."DateOnlyDateRange", r."DateTimeDateRange", r."DecimalRange", r."IntRange", r."LongRange", r."UserDefinedRange", r."UserDefinedRangeWithSchema"
FROM "RangeTestEntities" AS r
WHERE upper(r."UserDefinedRangeWithSchema")::double precision > 12.0
LIMIT 2
""");
    }

    #endregion

    #region Fixtures

    public class RangeQueryGaussDBFixture : SharedStoreFixtureBase<RangeContext>
    {
        static RangeQueryGaussDBFixture()
        {
            // TODO: Switch to using GaussDBDataSource
#pragma warning disable CS0618 // Type or member is obsolete
            GaussDBConnection.GlobalTypeMapper.EnableUnmappedTypes();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        protected override string StoreName
            => "RangeQueryTest";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override Task SeedAsync(RangeContext context)
            => RangeContext.SeedAsync(context);

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            var npgsqlOptionsBuilder = new GaussDBDbContextOptionsBuilder(optionsBuilder);
            npgsqlOptionsBuilder.MapRange("doublerange", typeof(double));
            npgsqlOptionsBuilder.MapRange<float>("Schema_Range", "test");
            return optionsBuilder;
        }
    }

    public class RangeTestEntity
    {
        public int Id { get; set; }
        public GaussDBRange<int> IntRange { get; set; }
        public GaussDBRange<long> LongRange { get; set; }
        public GaussDBRange<decimal> DecimalRange { get; set; }
        public GaussDBRange<DateOnly> DateOnlyDateRange { get; set; }

        [Column(TypeName = "tsrange")]
        public GaussDBRange<DateTime> DateTimeDateRange { get; set; }

        public GaussDBRange<double> UserDefinedRange { get; set; }
        public GaussDBRange<float> UserDefinedRangeWithSchema { get; set; }
    }

    public class RangeContext(DbContextOptions options) : PoolableDbContext(options)
    {
        public DbSet<RangeTestEntity> RangeTestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
            => builder.HasPostgresRange("doublerange", "double precision")
                .HasPostgresRange("test", "Schema_Range", "real");

        public static async Task SeedAsync(RangeContext context)
        {
            context.RangeTestEntities.AddRange(
                new RangeTestEntity
                {
                    Id = 1,
                    IntRange = new GaussDBRange<int>(1, 10),
                    LongRange = new GaussDBRange<long>(1, 10),
                    DecimalRange = new GaussDBRange<decimal>(1, 10),
                    DateOnlyDateRange = new GaussDBRange<DateOnly>(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 10)),
                    DateTimeDateRange = new GaussDBRange<DateTime>(new DateTime(2020, 1, 1), new DateTime(2020, 1, 10)),
                    UserDefinedRange = new GaussDBRange<double>(1, 10),
                    UserDefinedRangeWithSchema = new GaussDBRange<float>(1, 10)
                },
                new RangeTestEntity
                {
                    Id = 2,
                    IntRange = new GaussDBRange<int>(5, 15),
                    LongRange = new GaussDBRange<long>(5, 15),
                    DecimalRange = new GaussDBRange<decimal>(5, 15),
                    DateOnlyDateRange = new GaussDBRange<DateOnly>(new DateOnly(2020, 1, 5), new DateOnly(2020, 1, 15)),
                    DateTimeDateRange = new GaussDBRange<DateTime>(new DateTime(2020, 1, 5), new DateTime(2020, 1, 15)),
                    UserDefinedRange = new GaussDBRange<double>(5, 15),
                    UserDefinedRangeWithSchema = new GaussDBRange<float>(5, 15)
                });

            await context.SaveChangesAsync();
        }
    }

    #endregion

    #region Helpers

    protected RangeContext CreateContext()
        => Fixture.CreateContext();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    #endregion
}
