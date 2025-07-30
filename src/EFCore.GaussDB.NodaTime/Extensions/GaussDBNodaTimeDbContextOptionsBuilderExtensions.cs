using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     NodaTime specific extension methods for <see cref="GaussDBDbContextOptionsBuilder" />.
/// </summary>
public static class GaussDBNodaTimeDbContextOptionsBuilderExtensions
{
    /// <summary>
    ///     Configure NodaTime type mappings for Entity Framework.
    /// </summary>
    /// <returns> The options builder so that further configuration can be chained. </returns>
    public static GaussDBDbContextOptionsBuilder UseNodaTime(
        this GaussDBDbContextOptionsBuilder optionsBuilder)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;

        var extension = coreOptionsBuilder.Options.FindExtension<GaussDBNodaTimeOptionsExtension>()
            ?? new GaussDBNodaTimeOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}
