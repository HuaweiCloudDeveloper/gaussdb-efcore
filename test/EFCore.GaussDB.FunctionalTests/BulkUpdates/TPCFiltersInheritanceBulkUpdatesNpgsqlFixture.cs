namespace Microsoft.EntityFrameworkCore.BulkUpdates;

public class TPCFiltersInheritanceBulkUpdatesGaussDBFixture : TPCInheritanceBulkUpdatesGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
