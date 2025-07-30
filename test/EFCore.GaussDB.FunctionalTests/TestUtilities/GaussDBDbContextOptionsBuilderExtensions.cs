using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public static class GaussDBDbContextOptionsBuilderExtensions
{
    public static GaussDBDbContextOptionsBuilder ApplyConfiguration(this GaussDBDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);

        optionsBuilder.CommandTimeout(GaussDBTestStore.CommandTimeout);

        return optionsBuilder;
    }
}
