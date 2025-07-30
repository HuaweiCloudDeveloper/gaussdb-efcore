using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual GaussDBLTreeTranslator LTreeTranslator { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBMethodCallTranslatorProvider(
        RelationalMethodCallTranslatorProviderDependencies dependencies,
        IModel model,
        IDbContextOptions contextOptions)
        : base(dependencies)
    {
        var npgsqlOptions = contextOptions.FindExtension<GaussDBOptionsExtension>() ?? new GaussDBOptionsExtension();
        var supportsMultiranges = npgsqlOptions.PostgresVersion.AtLeast(14);
        var supportRegexCount = npgsqlOptions.PostgresVersion.AtLeast(15);

        var sqlExpressionFactory = (GaussDBExpressionFactory)dependencies.SqlExpressionFactory;
        var typeMappingSource = (GaussDBTypeMappingSource)dependencies.RelationalTypeMappingSource;
        var jsonTranslator = new GaussDBJsonPocoTranslator(typeMappingSource, sqlExpressionFactory, model);
        LTreeTranslator = new GaussDBLTreeTranslator(typeMappingSource, sqlExpressionFactory, model);

        AddTranslators(
        [
            new GaussDBArrayMethodTranslator(sqlExpressionFactory, jsonTranslator),
                new GaussDBByteArrayMethodTranslator(sqlExpressionFactory),
                new GaussDBConvertTranslator(sqlExpressionFactory),
                new GaussDBDateTimeMethodTranslator(typeMappingSource, sqlExpressionFactory),
                new GaussDBFullTextSearchMethodTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBFuzzyStringMatchMethodTranslator(sqlExpressionFactory),
                new GaussDBJsonDomTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBJsonDbFunctionsTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBJsonPocoTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBLikeTranslator(sqlExpressionFactory),
                LTreeTranslator,
                new GaussDBMathTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBNetworkTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBGuidTranslator(sqlExpressionFactory, npgsqlOptions.PostgresVersion),
                new GaussDBObjectToStringTranslator(typeMappingSource, sqlExpressionFactory),
                new GaussDBRandomTranslator(sqlExpressionFactory),
                new GaussDBRangeTranslator(typeMappingSource, sqlExpressionFactory, model, supportsMultiranges),
                new GaussDBRegexTranslator(typeMappingSource, sqlExpressionFactory, supportRegexCount),
                new GaussDBRowValueTranslator(sqlExpressionFactory),
                new GaussDBStringMethodTranslator(typeMappingSource, sqlExpressionFactory),
                new GaussDBTrigramsMethodTranslator(typeMappingSource, sqlExpressionFactory, model)
        ]);
    }
}
