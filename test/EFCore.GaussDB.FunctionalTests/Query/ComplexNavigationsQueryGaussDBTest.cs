using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsQueryGaussDBTest : ComplexNavigationsQueryRelationalTestBase<ComplexNavigationsQueryGaussDBFixture>
{
    private const string ApplySkip = "openGauss APPLY/LATERAL-style correlated projection translation emits invalid SQL";

    public ComplexNavigationsQueryGaussDBTest(ComplexNavigationsQueryGaussDBFixture fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Correlated_projection_with_first(bool async)
        => base.Correlated_projection_with_first(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Let_let_contains_from_outer_let(bool async)
        => base.Let_let_contains_from_outer_let(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Multiple_select_many_in_projection(bool async)
        => base.Multiple_select_many_in_projection(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(bool async)
        => base.Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Prune_does_not_throw_null_ref(bool async)
        => base.Prune_does_not_throw_null_ref(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Single_select_many_in_projection_with_take(bool async)
        => base.Single_select_many_in_projection_with_take(async);

    public override async Task Join_with_result_selector_returning_queryable_throws_validation_error(bool async)
        => await Assert.ThrowsAsync<ArgumentException>(
            () => base.Join_with_result_selector_returning_queryable_throws_validation_error(async));

    public override Task GroupJoin_client_method_in_OrderBy(bool async)
        => AssertTranslationFailedWithDetails(
            () => base.GroupJoin_client_method_in_OrderBy(async),
            CoreStrings.QueryUnableToTranslateMethod(
                "Microsoft.EntityFrameworkCore.Query.ComplexNavigationsQueryTestBase<Microsoft.EntityFrameworkCore.Query.ComplexNavigationsQueryGaussDBFixture>",
                "ClientMethodNullableInt"));
}
