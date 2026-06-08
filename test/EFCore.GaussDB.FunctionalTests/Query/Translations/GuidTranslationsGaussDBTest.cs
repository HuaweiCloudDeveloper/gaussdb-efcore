using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class GuidTranslationsGaussDBTest : GuidTranslationsTestBase<BasicTypesQueryGaussDBFixture>
{
    private const string BasicTypesDateOnlyMaterializationSkip =
        "openGauss currently materializes BasicTypesEntity.DateOnly via timestamp without time zone in this fixture, which the driver cannot read as DateOnly.";

    // ReSharper disable once UnusedParameter.Local
    public GuidTranslationsGaussDBTest(BasicTypesQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task New_with_constant()
    {
        await base.New_with_constant();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Guid" = 'df36f493-463f-4123-83f9-6b135deeb7ba'
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task New_with_parameter()
    {
        await base.New_with_parameter();

        AssertSql(
            """
@p='df36f493-463f-4123-83f9-6b135deeb7ba'

SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE b."Guid" = @p
""");
    }

    public override async Task ToString_projection()
    {
        await base.ToString_projection();

        AssertSql(
            """
SELECT b."Guid"::text
FROM "BasicTypesEntities" AS b
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public override async Task NewGuid()
    {
        await base.NewGuid();

        AssertSql(
            """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE uuid() <> '00000000-0000-0000-0000-000000000000'
""");
    }

    [ConditionalFact(Skip = BasicTypesDateOnlyMaterializationSkip)]
    public virtual async Task CreateVersion7()
    {
        await AssertQuery(
            ss => ss.Set<BasicTypesEntity>()
                .Where(od => Guid.CreateVersion7() != default));

        if (TestEnvironment.PostgresVersion >= new Version(18, 0))
        {
            AssertSql(
                """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
WHERE uuidv7() <> '00000000-0000-0000-0000-000000000000'
""");
        }
        else
        {
            AssertSql(
                """
SELECT b."Id", b."Bool", b."Byte", b."ByteArray", b."DateOnly", b."DateTime", b."DateTimeOffset", b."Decimal", b."Double", b."Enum", b."FlagsEnum", b."Float", b."Guid", b."Int", b."Long", b."Short", b."String", b."TimeOnly", b."TimeSpan"
FROM "BasicTypesEntities" AS b
""");
        }
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}

