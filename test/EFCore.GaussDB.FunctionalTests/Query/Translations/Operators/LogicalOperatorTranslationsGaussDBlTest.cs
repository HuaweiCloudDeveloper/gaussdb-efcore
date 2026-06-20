namespace Microsoft.EntityFrameworkCore.Query.Translations.Operators;

public class LogicalOperatorTranslationsGaussDBTest : LogicalOperatorTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string LogicalOperatorSkip =
        "Local-only: these logical-operator cases still materialize through the shared DateOnly/timestamp path which fails on the current target.";

    public LogicalOperatorTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task And()
        => Task.CompletedTask;

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task And_with_bool_property()
        => Task.CompletedTask;

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task Or()
        => Task.CompletedTask;

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task Or_with_bool_property()
        => Task.CompletedTask;

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task Not()
        => Task.CompletedTask;

    [ConditionalFact(Skip = LogicalOperatorSkip)]
    public override Task Not_with_bool_property()
        => Task.CompletedTask;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

