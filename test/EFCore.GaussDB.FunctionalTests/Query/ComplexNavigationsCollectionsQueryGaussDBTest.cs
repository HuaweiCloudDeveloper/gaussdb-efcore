namespace Microsoft.EntityFrameworkCore.Query;

public class ComplexNavigationsCollectionsQueryGaussDBTest : ComplexNavigationsCollectionsQueryRelationalTestBase<
    ComplexNavigationsQueryGaussDBFixture>
{
    public ComplexNavigationsCollectionsQueryGaussDBTest(
        ComplexNavigationsQueryGaussDBFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }
}
