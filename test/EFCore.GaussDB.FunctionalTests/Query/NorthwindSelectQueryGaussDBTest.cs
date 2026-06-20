namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindSelectQueryGaussDBTest : NorthwindSelectQueryRelationalTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string SelectSkip =
        "openGauss correlated collection, APPLY-style projection, and projection-shaping SQL diverge from provider expectations for these select queries.";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindSelectQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        ClearLog();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task Select_datetime_DayOfWeek_component(bool async)
    {
        await base.Select_datetime_DayOfWeek_component(async);

        AssertSql(
            """
SELECT floor(date_part('dow', o."OrderDate"))::int
FROM "Orders" AS o
""");
    }

    public override async Task Correlated_collection_after_distinct_with_complex_projection_not_containing_original_identifier(bool async)
    {
        // Identifier set for Distinct. Issue #24440.
        Assert.Equal(
            RelationalStrings.InsufficientInformationToIdentifyElementOfCollectionJoin,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Correlated_collection_after_distinct_with_complex_projection_not_containing_original_identifier(async)))
            .Message);

        AssertSql();
    }

    public override async Task
        SelectMany_with_collection_being_correlated_subquery_which_references_non_mapped_properties_from_inner_and_outer_entity(
            bool async)
    {
        await AssertUnableToTranslateEFProperty(
            () => base
                .SelectMany_with_collection_being_correlated_subquery_which_references_non_mapped_properties_from_inner_and_outer_entity(
                    async));

        AssertSql();
    }

    public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool async)
        => AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(async));

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Collection_projection_selecting_outer_element_followed_by_take(bool async)
        => base.Collection_projection_selecting_outer_element_followed_by_take(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Correlated_collection_after_distinct_not_containing_original_identifier(bool async)
        => base.Correlated_collection_after_distinct_not_containing_original_identifier(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Correlated_collection_after_distinct_with_complex_projection_containing_original_identifier(bool async)
        => base.Correlated_collection_after_distinct_with_complex_projection_containing_original_identifier(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Correlated_collection_after_groupby_with_complex_projection_containing_original_identifier(bool async)
        => base.Correlated_collection_after_groupby_with_complex_projection_containing_original_identifier(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(bool async)
        => base.Correlated_collection_after_groupby_with_complex_projection_not_containing_original_identifier(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Project_to_int_array(bool async)
        => base.Project_to_int_array(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Projecting_after_navigation_and_distinct(bool async)
        => base.Projecting_after_navigation_and_distinct(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Projection_when_arithmetic_expression_precedence(bool async)
        => base.Projection_when_arithmetic_expression_precedence(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Projection_when_arithmetic_expressions(bool async)
        => base.Projection_when_arithmetic_expressions(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Reverse_in_projection_subquery(bool async)
        => base.Reverse_in_projection_subquery(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Reverse_in_projection_subquery_single_result(bool async)
        => base.Reverse_in_projection_subquery_single_result(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Reverse_in_SelectMany_with_Take(bool async)
        => base.Reverse_in_SelectMany_with_Take(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Select_nested_collection_deep(bool async)
        => base.Select_nested_collection_deep(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Select_nested_collection_deep_distinct_no_identifiers(bool async)
        => base.Select_nested_collection_deep_distinct_no_identifiers(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_1(bool async)
        => base.SelectMany_correlated_with_outer_1(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_2(bool async)
        => base.SelectMany_correlated_with_outer_2(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_3(bool async)
        => base.SelectMany_correlated_with_outer_3(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_4(bool async)
        => base.SelectMany_correlated_with_outer_4(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_5(bool async)
        => base.SelectMany_correlated_with_outer_5(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_6(bool async)
        => base.SelectMany_correlated_with_outer_6(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_correlated_with_outer_7(bool async)
        => base.SelectMany_correlated_with_outer_7(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_whose_selector_references_outer_source(bool async)
        => base.SelectMany_whose_selector_references_outer_source(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(bool async)
        => base.SelectMany_with_collection_being_correlated_subquery_which_references_inner_and_outer_entity(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Set_operation_in_pending_collection(bool async)
        => base.Set_operation_in_pending_collection(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Take_on_correlated_collection_in_first(bool async)
        => base.Take_on_correlated_collection_in_first(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Take_on_top_level_and_on_collection_projection_with_outer_apply(bool async)
        => base.Take_on_top_level_and_on_collection_projection_with_outer_apply(async);

    [ConditionalTheory(Skip = SelectSkip)]
    public override Task Ternary_in_client_eval_assigns_correct_types(bool async)
        => base.Ternary_in_client_eval_assigns_correct_types(async);

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();
}
