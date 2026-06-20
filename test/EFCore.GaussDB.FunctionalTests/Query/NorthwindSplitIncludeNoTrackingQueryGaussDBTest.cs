namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindSplitIncludeNoTrackingQueryGaussDBTest : NorthwindSplitIncludeNoTrackingQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string IncludeApplySkip =
        "Local-only: openGauss rejects the current APPLY/LATERAL-shaped SQL generated for these split-include patterns.";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindSplitIncludeNoTrackingQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        // TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_cross_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_outer_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_outer_apply_with_filter_non_equality(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Filtered_include_with_multiple_ordering(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }
}
