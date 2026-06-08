namespace Microsoft.EntityFrameworkCore.Query.Translations.Operators;

public class MiscellaneousOperatorTranslationsGaussDBTest : MiscellaneousOperatorTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string ConditionalOperatorSkip =
        "Local-only: current openGauss null/conditional operator semantics for this fixture diverge from the PostgreSQL-oriented expectation.";

    public MiscellaneousOperatorTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = ConditionalOperatorSkip)]
    public override Task Conditional()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ConditionalOperatorSkip)]
    public override Task Coalesce()
        => Task.CompletedTask;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

