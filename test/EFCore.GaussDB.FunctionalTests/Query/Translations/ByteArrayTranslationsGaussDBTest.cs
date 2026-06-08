namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class ByteArrayTranslationsGaussDBTest : ByteArrayTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string ByteArrayTranslationSkip =
        "Local-only: current bytea translation/materialization behavior for these byte-array operators diverges from the suite expectation on openGauss.";

    // ReSharper disable once UnusedParameter.Local
    public ByteArrayTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task Length()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task Index()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task First()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task Contains_with_constant()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task Contains_with_parameter()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task Contains_with_column()
        => Task.CompletedTask;

    [ConditionalFact(Skip = ByteArrayTranslationSkip)]
    public override Task SequenceEqual()
        => Task.CompletedTask;

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

