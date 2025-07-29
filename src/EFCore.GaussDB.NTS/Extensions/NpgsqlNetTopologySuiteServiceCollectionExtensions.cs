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
public static class NpgsqlNetTopologySuiteServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the services required for NetTopologySuite support in the Npgsql provider for Entity Framework.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddEntityFrameworkNpgsqlNetTopologySuite(
        this IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        new EntityFrameworkNpgsqlServicesBuilder(serviceCollection)
            .TryAdd<INpgsqlDataSourceConfigurationPlugin, NetTopologySuiteDataSourceConfigurationPlugin>()
            .TryAdd<ISingletonOptions, INpgsqlNetTopologySuiteSingletonOptions>(p => p.GetRequiredService<INpgsqlNetTopologySuiteSingletonOptions>())
            .TryAdd<IRelationalTypeMappingSourcePlugin, NpgsqlNetTopologySuiteTypeMappingSourcePlugin>()
            .TryAdd<IMethodCallTranslatorPlugin, NpgsqlNetTopologySuiteMethodCallTranslatorPlugin>()
            .TryAdd<IAggregateMethodCallTranslatorPlugin, NpgsqlNetTopologySuiteAggregateMethodCallTranslatorPlugin>()
            .TryAdd<IMemberTranslatorPlugin, NpgsqlNetTopologySuiteMemberTranslatorPlugin>()
            .TryAdd<IConventionSetPlugin, NpgsqlNetTopologySuiteConventionSetPlugin>()
            .TryAddProviderSpecificServices(
                x => x.TryAddSingleton<INpgsqlNetTopologySuiteSingletonOptions, NpgsqlNetTopologySuiteSingletonOptions>());

        return serviceCollection;
    }
}
