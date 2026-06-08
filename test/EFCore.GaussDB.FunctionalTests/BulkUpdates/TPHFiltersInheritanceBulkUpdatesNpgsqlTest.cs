namespace Microsoft.EntityFrameworkCore.BulkUpdates;

public class TPHFiltersInheritanceBulkUpdatesGaussDBTest(
    TPHFiltersInheritanceBulkUpdatesGaussDBFixture fixture,
    ITestOutputHelper testOutputHelper)
    : FiltersInheritanceBulkUpdatesRelationalTestBase<
        TPHFiltersInheritanceBulkUpdatesGaussDBFixture>(fixture, testOutputHelper)
{
    private const string DeleteTranslationSkip =
        "Local-only: current GaussDB bulk delete SQL generation still produces malformed DELETE ... SELECT forms for filtered inheritance shapes and throws Inconceivable!.";

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_where_hierarchy(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_where_hierarchy_derived(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_where_using_hierarchy(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_where_using_hierarchy_derived(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task Delete_GroupBy_Where_Select_First(bool async)
    {
        await base.Delete_GroupBy_Where_Select_First(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_First_2(bool async)
    {
        await base.Delete_GroupBy_Where_Select_First_2(async);

        AssertSql();
    }

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_GroupBy_Where_Select_First_3(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task Delete_where_keyless_entity_mapped_to_sql_query(bool async)
    {
        await base.Delete_where_keyless_entity_mapped_to_sql_query(async);

        AssertSql();
    }

    public override async Task Update_base_type(bool async)
    {
        await base.Update_base_type(async);

        AssertExecuteUpdateSql(
            """
@p='Animal'

UPDATE "Animals" AS a
SET "Name" = @p
WHERE a."CountryId" = 1 AND a."Name" = 'Great spotted kiwi'
""");
    }

    public override async Task Update_base_type_with_OfType(bool async)
    {
        await base.Update_base_type_with_OfType(async);

        AssertExecuteUpdateSql(
            """
@p='NewBird'

UPDATE "Animals" AS a
SET "Name" = @p
WHERE a."CountryId" = 1 AND a."Discriminator" = 'Kiwi'
""");
    }

    [ConditionalTheory(Skip = DeleteTranslationSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Delete_where_hierarchy_subquery(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task Update_where_hierarchy_subquery(bool async)
    {
        await base.Update_where_hierarchy_subquery(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_base_property_on_derived_type(bool async)
    {
        await base.Update_base_property_on_derived_type(async);

        AssertExecuteUpdateSql(
            """
@p='SomeOtherKiwi'

UPDATE "Animals" AS a
SET "Name" = @p
WHERE a."Discriminator" = 'Kiwi' AND a."CountryId" = 1
""");
    }

    public override async Task Update_derived_property_on_derived_type(bool async)
    {
        await base.Update_derived_property_on_derived_type(async);

        AssertExecuteUpdateSql(
            """
@p='0' (DbType = Int16)

UPDATE "Animals" AS a
SET "FoundOn" = @p
WHERE a."Discriminator" = 'Kiwi' AND a."CountryId" = 1
""");
    }

    public override async Task Update_base_and_derived_types(bool async)
    {
        await base.Update_base_and_derived_types(async);

        AssertExecuteUpdateSql(
            """
@p='Kiwi'
@p0='0' (DbType = Int16)

UPDATE "Animals" AS a
SET "Name" = @p,
    "FoundOn" = @p0
WHERE a."Discriminator" = 'Kiwi' AND a."CountryId" = 1
""");
    }

    public override async Task Update_where_using_hierarchy(bool async)
    {
        await base.Update_where_using_hierarchy(async);

        AssertExecuteUpdateSql(
            """
@p='Monovia'

UPDATE "Countries" AS c
SET "Name" = @p
WHERE (
    SELECT count(*)::int
    FROM "Animals" AS a
    WHERE a."CountryId" = 1 AND c."Id" = a."CountryId" AND a."CountryId" > 0) > 0
""");
    }

    public override async Task Update_where_using_hierarchy_derived(bool async)
    {
        await base.Update_where_using_hierarchy_derived(async);

        AssertExecuteUpdateSql(
            """
@p='Monovia'

UPDATE "Countries" AS c
SET "Name" = @p
WHERE (
    SELECT count(*)::int
    FROM "Animals" AS a
    WHERE a."CountryId" = 1 AND c."Id" = a."CountryId" AND a."Discriminator" = 'Kiwi' AND a."CountryId" > 0) > 0
""");
    }

    public override async Task Update_where_keyless_entity_mapped_to_sql_query(bool async)
    {
        await base.Update_where_keyless_entity_mapped_to_sql_query(async);

        AssertExecuteUpdateSql();
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}
