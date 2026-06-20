namespace Microsoft.EntityFrameworkCore.Query;

public class TPCManyToManyNoTrackingQueryGaussDBTest : TPCManyToManyNoTrackingQueryRelationalTestBase<TPCManyToManyQueryGaussDBFixture>
{
    private const string LateralSubquerySkip =
        "Local-only: current GaussDB version rejects the LATERAL/APPLY-shaped SQL generated for these filtered skip-navigation include patterns.";

    public TPCManyToManyNoTrackingQueryGaussDBTest(TPCManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = LateralSubquerySkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where_EF_Property(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }
}
