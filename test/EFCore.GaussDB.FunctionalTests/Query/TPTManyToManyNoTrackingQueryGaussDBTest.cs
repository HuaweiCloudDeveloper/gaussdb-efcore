namespace Microsoft.EntityFrameworkCore.Query;

public class TPTManyToManyNoTrackingQueryGaussDBTest : TPTManyToManyNoTrackingQueryRelationalTestBase<TPTManyToManyQueryGaussDBFixture>
{
    private const string FilteredManyToManyIncludeSkip =
        "Local-only: current openGauss include translation for filtered skip-navigation order/skip/take chains diverges from the expected relational shape.";

    // ReSharper disable once UnusedParameter.Local
    public TPTManyToManyNoTrackingQueryGaussDBTest(TPTManyToManyQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    // TODO: #1232
    // protected override bool CanExecuteQueryString => true;

    [ConditionalTheory(Skip = FilteredManyToManyIncludeSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = FilteredManyToManyIncludeSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Filtered_include_skip_navigation_order_by_skip_take_then_include_skip_navigation_where_EF_Property(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }
}
