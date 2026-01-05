namespace Microsoft.EntityFrameworkCore.Query;

public class CompositeKeysSplitQueryGaussDBTest : CompositeKeysSplitQueryRelationalTestBase<CompositeKeysQueryGaussDBFixture>
{
    public CompositeKeysSplitQueryGaussDBTest(
        CompositeKeysQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
