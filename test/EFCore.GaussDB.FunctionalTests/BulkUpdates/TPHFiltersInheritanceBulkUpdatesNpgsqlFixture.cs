namespace Microsoft.EntityFrameworkCore.BulkUpdates;

public class TPHFiltersInheritanceBulkUpdatesGaussDBFixture : TPHInheritanceBulkUpdatesGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
