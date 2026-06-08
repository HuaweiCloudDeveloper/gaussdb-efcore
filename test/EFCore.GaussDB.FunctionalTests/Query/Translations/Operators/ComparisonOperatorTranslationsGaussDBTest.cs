namespace Microsoft.EntityFrameworkCore.Query.Translations.Operators;

public class ComparisonOperatorTranslationsGaussDBTest : ComparisonOperatorTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string BasicTypesDateOnlyMaterializationSkip =
        "openGauss currently materializes BasicTypesEntity.DateOnly via timestamp without time zone in this fixture, which the driver cannot read as DateOnly.";

    public ComparisonOperatorTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task Equal()
    {
        await base.Equal();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" = 8
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task NotEqual()
    {
        await base.NotEqual();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" <> 8
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task GreaterThan()
    {
        await base.GreaterThan();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" > 8
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task GreaterThanOrEqual()
    {
        await base.GreaterThanOrEqual();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" >= 8
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task LessThan()
    {
        await base.LessThan();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" < 8
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task LessThanOrEqual()
    {
        await base.LessThanOrEqual();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Int" <= 8
""");
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

