namespace Microsoft.EntityFrameworkCore.BulkUpdates;

public class TPTFiltersInheritanceBulkUpdatesGaussDBFixture : TPTInheritanceBulkUpdatesGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
