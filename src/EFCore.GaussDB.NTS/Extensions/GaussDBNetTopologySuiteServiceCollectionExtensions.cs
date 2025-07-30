using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Query.ExpressionTranslators.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     HuaweiCloud.EntityFrameworkCore.GaussDB.NetTopologySuite extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class GaussDBNetTopologySuiteServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the services required for NetTopologySuite support in the GaussDB provider for Entity Framework.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddEntityFrameworkGaussDBNetTopologySuite(
        this IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        new EntityFrameworkGaussDBServicesBuilder(serviceCollection)
            .TryAdd<IGaussDBDataSourceConfigurationPlugin, NetTopologySuiteDataSourceConfigurationPlugin>()
            .TryAdd<ISingletonOptions, IGaussDBNetTopologySuiteSingletonOptions>(p => p.GetRequiredService<IGaussDBNetTopologySuiteSingletonOptions>())
            .TryAdd<IRelationalTypeMappingSourcePlugin, GaussDBNetTopologySuiteTypeMappingSourcePlugin>()
            .TryAdd<IMethodCallTranslatorPlugin, GaussDBNetTopologySuiteMethodCallTranslatorPlugin>()
            .TryAdd<IAggregateMethodCallTranslatorPlugin, GaussDBNetTopologySuiteAggregateMethodCallTranslatorPlugin>()
            .TryAdd<IMemberTranslatorPlugin, GaussDBNetTopologySuiteMemberTranslatorPlugin>()
            .TryAdd<IConventionSetPlugin, GaussDBNetTopologySuiteConventionSetPlugin>()
            .TryAddProviderSpecificServices(
                x => x.TryAddSingleton<IGaussDBNetTopologySuiteSingletonOptions, GaussDBNetTopologySuiteSingletonOptions>());

        return serviceCollection;
    }
}
