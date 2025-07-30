namespace Microsoft.EntityFrameworkCore.BulkUpdates;

public class TPCInheritanceBulkUpdatesGaussDBFixture : TPCInheritanceBulkUpdatesFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    public override bool UseGeneratedKeys
        => false;
}
