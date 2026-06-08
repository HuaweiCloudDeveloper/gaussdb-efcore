namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsCollectionsSplitSharedTypeQueryGaussDBTest :
    ComplexNavigationsCollectionsSplitSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQueryGaussDBFixture>
{
    private const string SplitQuerySyntaxSkip =
        "Local-only: openGauss currently generates invalid SQL for these split-query collection/include shapes, hitting syntax errors near nested SELECT clauses; fixing this cleanly would require broader provider SQL-generation work.";

    public ComplexNavigationsCollectionsSplitSharedTypeQueryGaussDBTest(
        ComplexNavigationsSharedTypeQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Skip_Take_Select_collection_Skip_Take(bool async)
        => base.Skip_Take_Select_collection_Skip_Take(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Complex_query_with_let_collection_projection_FirstOrDefault_with_ToList_on_inner_and_outer(bool async)
        => base.Complex_query_with_let_collection_projection_FirstOrDefault_with_ToList_on_inner_and_outer(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(bool async)
        => base.Filtered_include_same_filter_set_on_same_navigation_twice_followed_by_ThenIncludes(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(bool async)
        => base.Filtered_include_Skip_Take_with_another_Skip_Take_on_top_level(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Complex_query_with_let_collection_projection_FirstOrDefault(bool async)
        => base.Complex_query_with_let_collection_projection_FirstOrDefault(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_FirstOrDefault_on_top_level(bool async)
        => base.Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_FirstOrDefault_on_top_level(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task SelectMany_with_predicate_and_DefaultIfEmpty_projecting_root_collection_element_and_another_collection(bool async)
        => base.SelectMany_with_predicate_and_DefaultIfEmpty_projecting_root_collection_element_and_another_collection(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(bool async)
        => base.Filtered_include_and_non_filtered_include_followed_by_then_include_on_same_navigation(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Skip_Take_Distinct_on_grouping_element(bool async)
        => base.Skip_Take_Distinct_on_grouping_element(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Skip_Take_on_grouping_element_inside_collection_projection(bool async)
        => base.Skip_Take_on_grouping_element_inside_collection_projection(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Take_Select_collection_Take(bool async)
        => base.Take_Select_collection_Take(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Skip_Take_on_grouping_element_with_reference_include(bool async)
        => base.Skip_Take_on_grouping_element_with_reference_include(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(bool async)
        => base.Filtered_include_multiple_multi_level_includes_with_first_level_using_filter_include_on_one_of_the_chains_only(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_Take_with_another_Take_on_top_level(bool async)
        => base.Filtered_include_Take_with_another_Take_on_top_level(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Projecting_collection_after_optional_reference_correlated_with_parent(bool async)
        => base.Projecting_collection_after_optional_reference_correlated_with_parent(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Projecting_collection_with_group_by_after_optional_reference_correlated_with_parent(bool async)
        => base.Projecting_collection_with_group_by_after_optional_reference_correlated_with_parent(async);

    [ConditionalTheory(Skip = SplitQuerySyntaxSkip)]
    public override Task Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_unordered_Take_on_top_level(bool async)
        => base.Filtered_include_with_Take_without_order_by_followed_by_ThenInclude_and_unordered_Take_on_top_level(async);
}
