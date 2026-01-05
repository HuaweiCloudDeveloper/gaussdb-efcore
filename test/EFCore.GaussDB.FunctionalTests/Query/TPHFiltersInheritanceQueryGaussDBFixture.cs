namespace Microsoft.EntityFrameworkCore.Query;

public class TPHFiltersInheritanceQueryGaussDBFixture : TPHInheritanceQueryGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
