namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBAggregateMethodCallTranslatorProvider : RelationalAggregateMethodCallTranslatorProvider
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBAggregateMethodCallTranslatorProvider(
        RelationalAggregateMethodCallTranslatorProviderDependencies dependencies,
        IModel model)
        : base(dependencies)
    {
        var sqlExpressionFactory = (GaussDBExpressionFactory)dependencies.SqlExpressionFactory;
        var typeMappingSource = dependencies.RelationalTypeMappingSource;

        AddTranslators(
        [
            new GaussDBQueryableAggregateMethodTranslator(sqlExpressionFactory, typeMappingSource),
                new GaussDBStatisticsAggregateMethodTranslator(sqlExpressionFactory, typeMappingSource),
                new GaussDBMiscAggregateMethodTranslator(sqlExpressionFactory, typeMappingSource, model)
        ]);
    }
}
