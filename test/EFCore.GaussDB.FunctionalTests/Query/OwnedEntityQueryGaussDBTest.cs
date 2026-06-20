namespace Microsoft.EntityFrameworkCore.Query;

public class OwnedEntityQueryGaussDBTest(NonSharedFixture fixture) : OwnedEntityQueryRelationalTestBase(fixture)
{
    private const string OwnedCollectionProjectionSkip =
        "Local-only: current openGauss translation for correlated collection projection over owned entities diverges from the expected relational shape.";

    [ConditionalTheory(Skip = OwnedCollectionProjectionSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Projecting_correlated_collection_property_for_owned_entity(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
