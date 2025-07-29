using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime.Query.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class NpgsqlNodaTimeServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the services required for NodaTime support in the Npgsql provider for Entity Framework.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddEntityFrameworkNpgsqlNodaTime(
        this IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        new EntityFrameworkNpgsqlServicesBuilder(serviceCollection)
            .TryAdd<INpgsqlDataSourceConfigurationPlugin, NodaTimeDataSourceConfigurationPlugin>()
            .TryAdd<IRelationalTypeMappingSourcePlugin, NpgsqlNodaTimeTypeMappingSourcePlugin>()
            .TryAdd<IMethodCallTranslatorPlugin, NpgsqlNodaTimeMethodCallTranslatorPlugin>()
            .TryAdd<IAggregateMethodCallTranslatorPlugin, NpgsqlNodaTimeAggregateMethodCallTranslatorPlugin>()
            .TryAdd<IMemberTranslatorPlugin, NpgsqlNodaTimeMemberTranslatorPlugin>()
            .TryAdd<IEvaluatableExpressionFilterPlugin, NpgsqlNodaTimeEvaluatableExpressionFilterPlugin>();

        return serviceCollection;
    }
}
