using Microsoft.EntityFrameworkCore.TestModels.NodaTime;
using NodaTime;

namespace Microsoft.EntityFrameworkCore.Query.Translations.NodaTime;

public class LocalTimeTranslationsTest : QueryTestBase<NodaTimeQueryGaussDBFixture>
{
    private const string LocalTimeMaterializationSkip =
        "Local-only: openGauss currently materializes the NodaTime fixture through timestamp without time zone shapes that the driver cannot read back as LocalDate-backed values; fixing this cleanly would require broader provider work.";

    public LocalTimeTranslationsTest(NodaTimeQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = LocalTimeMaterializationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public async Task Subtract_LocalTime(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<NodaTimeTypes>().Where(t => t.LocalTime + Period.FromHours(1) - t.LocalTime == Period.FromHours(1)));

        AssertSql(
            """
SELECT n."Id", n."DateInterval", n."Duration", n."Instant", n."InstantRange", n."Interval", n."LocalDate", n."LocalDate2", n."LocalDateRange", n."LocalDateTime", n."LocalTime", n."Long", n."OffsetTime", n."Period", n."TimeZoneId", n."ZonedDateTime"
FROM "NodaTimeTypes" AS n
WHERE (n."LocalTime" + INTERVAL 'PT1H') - n."LocalTime" = INTERVAL 'PT1H'
""");
    }

    [ConditionalTheory(Skip = LocalTimeMaterializationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public async Task Hour(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<NodaTimeTypes>().Where(t => t.LocalTime.Hour == 10));

        AssertSql(
            """
SELECT n."Id", n."DateInterval", n."Duration", n."Instant", n."InstantRange", n."Interval", n."LocalDate", n."LocalDate2", n."LocalDateRange", n."LocalDateTime", n."LocalTime", n."Long", n."OffsetTime", n."Period", n."TimeZoneId", n."ZonedDateTime"
FROM "NodaTimeTypes" AS n
WHERE date_part('hour', n."LocalTime")::int = 10
""");
    }

    [ConditionalTheory(Skip = LocalTimeMaterializationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public async Task Minute(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<NodaTimeTypes>().Where(t => t.LocalTime.Minute == 31));

        AssertSql(
            """
SELECT n."Id", n."DateInterval", n."Duration", n."Instant", n."InstantRange", n."Interval", n."LocalDate", n."LocalDate2", n."LocalDateRange", n."LocalDateTime", n."LocalTime", n."Long", n."OffsetTime", n."Period", n."TimeZoneId", n."ZonedDateTime"
FROM "NodaTimeTypes" AS n
WHERE date_part('minute', n."LocalTime")::int = 31
""");
    }

    [ConditionalTheory(Skip = LocalTimeMaterializationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public async Task Second(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<NodaTimeTypes>().Where(t => t.LocalTime.Second == 33));

        AssertSql(
            """
SELECT n."Id", n."DateInterval", n."Duration", n."Instant", n."InstantRange", n."Interval", n."LocalDate", n."LocalDate2", n."LocalDateRange", n."LocalDateTime", n."LocalTime", n."Long", n."OffsetTime", n."Period", n."TimeZoneId", n."ZonedDateTime"
FROM "NodaTimeTypes" AS n
WHERE floor(date_part('second', n."LocalTime"))::int = 33
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
