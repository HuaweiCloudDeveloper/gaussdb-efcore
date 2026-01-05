namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsCollectionsSharedTypeQueryGaussDBTest : ComplexNavigationsCollectionsSharedTypeQueryRelationalTestBase<
    ComplexNavigationsSharedTypeQueryGaussDBFixture>
{
    public ComplexNavigationsCollectionsSharedTypeQueryGaussDBTest(
        ComplexNavigationsSharedTypeQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
