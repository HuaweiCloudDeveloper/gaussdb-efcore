namespace Microsoft.EntityFrameworkCore.Query;

public class CompositeKeysQueryGaussDBTest : CompositeKeysQueryRelationalTestBase<CompositeKeysQueryGaussDBFixture>
{
    public CompositeKeysQueryGaussDBTest(
        CompositeKeysQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
