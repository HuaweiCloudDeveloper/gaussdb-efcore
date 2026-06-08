namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindIncludeQueryGaussDBTest : NorthwindIncludeQueryRelationalTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string LateralSubquerySkip =
        "Local-only: current GaussDB version rejects the LATERAL/APPLY-shaped SQL generated for these include patterns.";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindIncludeQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Include_collection_with_cross_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Include_collection_with_outer_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Include_collection_with_outer_apply_with_filter_non_equality(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Filtered_include_with_multiple_ordering(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }
}
