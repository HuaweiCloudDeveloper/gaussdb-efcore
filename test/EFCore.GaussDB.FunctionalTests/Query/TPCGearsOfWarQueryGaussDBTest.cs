using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;

namespace Microsoft.EntityFrameworkCore.Query;

public class TPCGearsOfWarQueryGaussDBTest : TPCGearsOfWarQueryRelationalTestBase<TPCGearsOfWarQueryGaussDBFixture>
{
    private const string QueryShapeSkip =
        "openGauss provider still has gaps for correlated-collection SQL, nullability processing, and DateOnly materialization in these GearsOfWar query shapes";

    public TPCGearsOfWarQueryGaussDBTest(TPCGearsOfWarQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_after_distinct_3_levels(bool async)
        => base.Correlated_collection_after_distinct_3_levels(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(bool async)
        => base.Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_distinct_not_projecting_identifier_column(bool async)
        => base.Correlated_collection_with_distinct_not_projecting_identifier_column(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_distinct_projecting_identifier_column(bool async)
        => base.Correlated_collection_with_distinct_projecting_identifier_column(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(bool async)
        => base.Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
        => base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(bool async)
        => base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
        => base.Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collection_with_inner_collection_references_element_two_levels_up(bool async)
        => base.Correlated_collection_with_inner_collection_references_element_two_levels_up(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool async)
        => base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool async)
        => base.Correlated_collections_inner_subquery_selector_references_outer_qsre(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool async)
        => base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool async)
        => base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Correlated_collections_with_Distinct(bool async)
        => base.Correlated_collections_with_Distinct(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Optional_navigation_type_compensation_works_with_array_initializers(bool async)
        => base.Optional_navigation_type_compensation_works_with_array_initializers(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool async)
        => base.Outer_parameter_in_group_join_with_DefaultIfEmpty(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Outer_parameter_in_join_key(bool async)
        => base.Outer_parameter_in_join_key(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Outer_parameter_in_join_key_inner_and_outer(bool async)
        => base.Outer_parameter_in_join_key_inner_and_outer(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Projecting_property_converted_to_nullable_into_new_array(bool async)
        => base.Projecting_property_converted_to_nullable_into_new_array(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Select_null_propagation_negative4(bool async)
        => base.Select_null_propagation_negative4(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Select_null_propagation_optimization8(bool async)
        => base.Select_null_propagation_optimization8(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(bool async)
        => base.SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(bool async)
        => base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(bool async)
        => base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
        => base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(bool async)
        => base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task ToString_boolean_computed_nullable(bool async)
        => base.ToString_boolean_computed_nullable(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task ToString_boolean_property_nullable(bool async)
        => base.ToString_boolean_property_nullable(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task ToString_nullable_enum_property_projection(bool async)
        => base.ToString_nullable_enum_property_projection(async);

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override Task Where_equals_method_on_nullable_with_object_overload(bool async)
        => base.Where_equals_method_on_nullable_with_object_overload(async);

    // Base implementation uses DateTimeOffset.Now, which we don't translate by design. Use DateTimeOffset.UtcNow instead.
    public override async Task Select_datetimeoffset_comparison_in_projection(bool async)
    {
        await AssertQueryScalar(
            async,
            ss => ss.Set<Mission>().Select(m => m.Timeline > DateTimeOffset.UtcNow));

        AssertSql(
            """
SELECT m."Timeline" > now()
FROM "Missions" AS m
""");
    }

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override async Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
    {
        var dto = new DateTimeOffset(599898024001234567, new TimeSpan(0));
        var start = dto.AddDays(-1);
        var end = dto.AddDays(1);
        var dates = new[] { dto };

        await AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(
                m => start <= m.Timeline.Date && m.Timeline < end && dates.Contains(m.Timeline)),
            assertEmpty: true); // TODO: Check this out

        AssertSql(
            """
@start='1902-01-01T10:00:00.1234567+00:00' (DbType = DateTime)
@end='1902-01-03T10:00:00.1234567+00:00' (DbType = DateTime)
@dates={ '1902-01-02T10:00:00.1234567+00:00' } (DbType = Object)

SELECT m."Id", m."CodeName", m."Date", m."Difficulty", m."Duration", m."Rating", m."Time", m."Timeline"
FROM "Missions" AS m
WHERE @start <= date_trunc('day', m."Timeline" AT TIME ZONE 'UTC')::timestamptz AND m."Timeline" < @end AND m."Timeline" = ANY (@dates)
""");
    }

    [ConditionalTheory(Skip = QueryShapeSkip)]
    public override async Task DateTimeOffset_Date_returns_datetime(bool async)
    {
        var dateTimeOffset = new DateTimeOffset(2, 3, 1, 8, 0, 0, new TimeSpan(-5, 0, 0));

        await AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(m => m.Timeline.Date.ToLocalTime() >= dateTimeOffset.Date));

        AssertSql(
            """
@dateTimeOffset_Date='0002-03-01T00:00:00.0000000'

SELECT m."Id", m."CodeName", m."Date", m."Difficulty", m."Duration", m."Rating", m."Time", m."Timeline"
FROM "Missions" AS m
WHERE date_trunc('day', m."Timeline" AT TIME ZONE 'UTC')::timestamp >= @dateTimeOffset_Date
""");
    }

    // Not supported by design: we support getting a local DateTime via DateTime.Now (based on PG TimeZone), but there's no way to get a
    // non-UTC DateTimeOffset.
    public override Task DateTimeOffsetNow_minus_timespan(bool async)
        => Assert.ThrowsAsync<InvalidOperationException>(() => base.DateTimeOffsetNow_minus_timespan(async));

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
