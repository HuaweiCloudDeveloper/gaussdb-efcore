namespace Microsoft.EntityFrameworkCore.Query.Translations.Operators;

public class MiscellaneousOperatorTranslationsGaussDBTest : MiscellaneousOperatorTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    public MiscellaneousOperatorTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Conditional(bool async)
    {
        await base.Conditional(async);

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE CASE
    WHEN b."Int" = 8 THEN b."String"
    ELSE 'Foo'
END = 'Seattle'
""");
    }

    public override async Task Coalesce(bool async)
    {
        await base.Coalesce(async);

        AssertSql(
            """
SELECT n."Id", n."Bool", n."Byte", n."ByteArray", n."DateOnly", n."DateTime", n."DateTimeOffset", n."Decimal", n."Double", n."Enum", n."FlagsEnum", n."Float", n."Guid", n."Int", n."Long", n."Short", n."String", n."TimeOnly", n."TimeSpan"
FROM "NullableBasicTypesEntities" AS n
WHERE COALESCE(n."String", 'Unknown') = 'Seattle'
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
