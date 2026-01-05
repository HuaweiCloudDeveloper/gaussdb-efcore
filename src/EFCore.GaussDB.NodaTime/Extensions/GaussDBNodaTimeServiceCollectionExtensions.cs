using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime.Query.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class GaussDBNodaTimeServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the services required for NodaTime support in the GaussDB provider for Entity Framework.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddEntityFrameworkGaussDBNodaTime(
        this IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        new EntityFrameworkGaussDBServicesBuilder(serviceCollection)
            .TryAdd<IGaussDBDataSourceConfigurationPlugin, NodaTimeDataSourceConfigurationPlugin>()
            .TryAdd<IRelationalTypeMappingSourcePlugin, GaussDBNodaTimeTypeMappingSourcePlugin>()
            .TryAdd<IMethodCallTranslatorPlugin, GaussDBNodaTimeMethodCallTranslatorPlugin>()
            .TryAdd<IAggregateMethodCallTranslatorPlugin, GaussDBNodaTimeAggregateMethodCallTranslatorPlugin>()
            .TryAdd<IMemberTranslatorPlugin, GaussDBNodaTimeMemberTranslatorPlugin>()
            .TryAdd<IEvaluatableExpressionFilterPlugin, GaussDBNodaTimeEvaluatableExpressionFilterPlugin>();

        return serviceCollection;
    }
}
