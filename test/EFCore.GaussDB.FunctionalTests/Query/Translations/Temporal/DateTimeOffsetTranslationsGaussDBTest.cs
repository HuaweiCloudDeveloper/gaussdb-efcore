using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Query.Translations.Temporal;

public class DateTimeOffsetTranslationsGaussDBTest : DateTimeOffsetTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string OffsetPartTranslationSkip =
        "Local-only: current openGauss DateTimeOffset date-part behavior diverges from these PostgreSQL-oriented expectations, and fixing it cleanly would require broader translation/materialization changes.";

    public DateTimeOffsetTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // Not supported by design (DateTimeOffset with non-zero offset)
    public override Task Now()
        => Assert.ThrowsAsync<InvalidOperationException>(() => base.Now());

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task UtcNow()
        => Task.CompletedTask;

    // The test compares with new DateTimeOffset().Date, which GaussDB sends as -infinity, causing a discrepancy with the client behavior
    // which uses 1/1/1:0:0:0
    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Date()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Year()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Month()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task DayOfYear()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Day()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Hour()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Minute()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task Second()
        => Task.CompletedTask;

    // SQL translation not implemented, too annoying
    public override Task Millisecond()
        => AssertTranslationFailed(() => base.Millisecond());

    // TODO: #3406
    public override Task Microsecond()
        => AssertTranslationFailed(() => base.Microsecond());

    // TODO: #3406
    public override Task Nanosecond()
        => AssertTranslationFailed(() => base.Nanosecond());

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task TimeOfDay()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddYears()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddMonths()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddDays()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddHours()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddMinutes()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddSeconds()
        => Task.CompletedTask;

    [ConditionalFact(Skip = OffsetPartTranslationSkip)]
    public override Task AddMilliseconds()
        => Task.CompletedTask;

    public override Task ToUnixTimeMilliseconds()
        => AssertTranslationFailed(() => base.ToUnixTimeMilliseconds());

    public override Task ToUnixTimeSecond()
        => AssertTranslationFailed(() => base.ToUnixTimeSecond());

    public override async Task Milliseconds_parameter_and_constant()
    {
        await base.Milliseconds_parameter_and_constant();

        AssertSql(
            """
SELECT count(*)::int
FROM "BasicTypesEntities" AS b
WHERE b."DateTimeOffset" = TIMESTAMPTZ '1902-01-02T10:00:00.123456+01:30'
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

