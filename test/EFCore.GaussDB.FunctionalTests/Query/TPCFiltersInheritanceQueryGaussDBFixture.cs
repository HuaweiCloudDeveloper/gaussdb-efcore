namespace Microsoft.EntityFrameworkCore.Query;

public class TPCFiltersInheritanceQueryGaussDBFixture : TPCInheritanceQueryGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
