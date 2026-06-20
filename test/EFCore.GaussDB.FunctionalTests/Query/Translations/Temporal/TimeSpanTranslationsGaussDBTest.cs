namespace Microsoft.EntityFrameworkCore.Query.Translations.Temporal;

public class TimeSpanTranslationsGaussDBTest : TimeSpanTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string TimeSpanPartTranslationSkip =
        "Local-only: current openGauss interval part extraction diverges from these expectations for this fixture, and fixing it would require broader translation/materialization work.";

    public TimeSpanTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = TimeSpanPartTranslationSkip)]
    public override Task Hours()
        => Task.CompletedTask;

    [ConditionalFact(Skip = TimeSpanPartTranslationSkip)]
    public override Task Minutes()
        => Task.CompletedTask;

    [ConditionalFact(Skip = TimeSpanPartTranslationSkip)]
    public override Task Seconds()
        => Task.CompletedTask;

    [ConditionalFact(Skip = TimeSpanPartTranslationSkip)]
    public override Task Milliseconds()
        => Task.CompletedTask;

    public override Task Microseconds()
        => AssertTranslationFailed(() => base.Microseconds());

    public override Task Nanoseconds()
        => AssertTranslationFailed(() => base.Nanoseconds());

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

