namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindIncludeNoTrackingQueryGaussDBTest : NorthwindIncludeNoTrackingQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
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
}
