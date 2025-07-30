namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsCollectionsSplitQueryGaussDBTest : ComplexNavigationsCollectionsSplitQueryRelationalTestBase<
    ComplexNavigationsQueryGaussDBFixture>
{
    public ComplexNavigationsCollectionsSplitQueryGaussDBTest(
        ComplexNavigationsQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
