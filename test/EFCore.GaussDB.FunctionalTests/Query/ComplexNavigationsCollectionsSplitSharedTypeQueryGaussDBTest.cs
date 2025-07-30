namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsCollectionsSplitSharedTypeQueryGaussDBTest :
    ComplexNavigationsCollectionsSplitSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQueryGaussDBFixture>
{
    public ComplexNavigationsCollectionsSplitSharedTypeQueryGaussDBTest(
        ComplexNavigationsSharedTypeQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
