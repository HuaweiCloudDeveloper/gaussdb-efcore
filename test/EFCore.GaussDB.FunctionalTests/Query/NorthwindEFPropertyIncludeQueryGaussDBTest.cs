namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindEFPropertyIncludeQueryGaussDBTest : NorthwindEFPropertyIncludeQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindEFPropertyIncludeQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public override async Task Include_collection_with_last_no_orderby(bool async)
    {
        Assert.Equal(
            RelationalStrings.LastUsedWithoutOrderBy(nameof(Enumerable.Last)),
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Include_collection_with_last_no_orderby(async))).Message);

        AssertSql();
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
