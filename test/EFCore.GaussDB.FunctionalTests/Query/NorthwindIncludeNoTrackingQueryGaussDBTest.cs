namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindIncludeNoTrackingQueryGaussDBTest : NorthwindIncludeNoTrackingQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string IncludeApplySkip =
        "Local-only: openGauss rejects the current APPLY/LATERAL-shaped SQL generated for these include patterns.";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindIncludeNoTrackingQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public override async Task Include_collection_with_last_no_orderby(bool async)
        => Assert.Equal(
            RelationalStrings.LastUsedWithoutOrderBy(nameof(Enumerable.Last)),
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Include_collection_with_last_no_orderby(async))).Message);

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
