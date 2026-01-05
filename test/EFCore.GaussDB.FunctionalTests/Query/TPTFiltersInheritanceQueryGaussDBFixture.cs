namespace Microsoft.EntityFrameworkCore.Query;

public class TPTFiltersInheritanceQuerySqlServerFixture : TPTInheritanceQueryGaussDBFixture
{
    public override bool EnableFilters
        => true;
}
