namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindJoinQueryGaussDBTest : NorthwindJoinQueryRelationalTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string ApplySkip = "openGauss APPLY/LATERAL-style SelectMany translation emits invalid SQL";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindJoinQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        ClearLog();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // #2759
    public override Task Join_local_collection_int_closure_is_cached_correctly(bool async)
        => base.Join_local_collection_int_closure_is_cached_correctly(async);
    // => Assert.ThrowsAsync<InvalidOperationException>(() => base.Join_local_collection_int_closure_is_cached_correctly(async));

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_client_eval(bool async)
        => base.SelectMany_with_client_eval(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_client_eval_with_collection_shaper(bool async)
        => base.SelectMany_with_client_eval_with_collection_shaper(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_client_eval_with_collection_shaper_ignored(bool async)
        => base.SelectMany_with_client_eval_with_collection_shaper_ignored(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_selecting_outer_element(bool async)
        => base.SelectMany_with_selecting_outer_element(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_selecting_outer_entity(bool async)
        => base.SelectMany_with_selecting_outer_entity(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task SelectMany_with_selecting_outer_entity_column_and_inner_column(bool async)
        => base.SelectMany_with_selecting_outer_entity_column_and_inner_column(async);

    [ConditionalTheory(Skip = ApplySkip)]
    public override Task Take_in_collection_projection_with_FirstOrDefault_on_top_level(bool async)
        => base.Take_in_collection_projection_with_FirstOrDefault_on_top_level(async);

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();
}
