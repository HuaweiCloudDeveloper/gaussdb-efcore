using HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;
using static HuaweiCloud.EntityFrameworkCore.GaussDB.Utilities.Statics;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBJsonDbFunctionsTranslator : IMethodCallTranslator
{
    private readonly GaussDBExpressionFactory _sqlExpressionFactory;
    private readonly RelationalTypeMapping _stringTypeMapping;
    private readonly RelationalTypeMapping _jsonbTypeMapping;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBJsonDbFunctionsTranslator(
        IRelationalTypeMappingSource typeMappingSource,
        GaussDBExpressionFactory sqlExpressionFactory,
        IModel model)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _stringTypeMapping = typeMappingSource.FindMapping(typeof(string), model)!;
        _jsonbTypeMapping = typeMappingSource.FindMapping("jsonb")!;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method.DeclaringType != typeof(GaussDBJsonDbFunctionsExtensions))
        {
            return null;
        }

        var args = arguments
            // Skip useless DbFunctions instance
            .Skip(1)
            // JSON extensions accept object parameters for JSON, since they must be able to handle POCOs, strings or DOM types.
            // This means they come wrapped in a convert node, which we need to remove.
            // Convert nodes may also come from wrapping JsonTraversalExpressions generated through POCO traversal.
            .Select(RemoveConvert)
            // If a function is invoked over a JSON traversal expression, that expression may come with
            // returnText: true (i.e. operator ->> and not ->). Since the functions below require a json object and
            // not text, we transform it.
            .Select(a => a is GaussDBJsonTraversalExpression traversal ? WithReturnsText(traversal, false) : a)
            .ToArray();

        if (!args.Any(a => a.TypeMapping is GaussDBJsonTypeMapping || a is GaussDBJsonTraversalExpression))
        {
            throw new InvalidOperationException("The EF JSON methods require a JSON parameter and none was found.");
        }

        if (method.Name == nameof(GaussDBJsonDbFunctionsExtensions.JsonTypeof))
        {
            return _sqlExpressionFactory.Function(
                ((GaussDBJsonTypeMapping)args[0].TypeMapping!).IsJsonb ? "jsonb_typeof" : "json_typeof",
                [args[0]],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(string));
        }

        // The following are jsonb-only, not support on json
        if (args.Any(a => a.TypeMapping is GaussDBJsonTypeMapping { IsJsonb: false }))
        {
            throw new InvalidOperationException("JSON methods on EF.Functions only support the jsonb type, not json.");
        }

        return method.Name switch
        {
            nameof(GaussDBJsonDbFunctionsExtensions.JsonContains)
                => _sqlExpressionFactory.Contains(Jsonb(args[0]), Jsonb(args[1])),
            nameof(GaussDBJsonDbFunctionsExtensions.JsonContained)
                => _sqlExpressionFactory.ContainedBy(Jsonb(args[0]), Jsonb(args[1])),
            nameof(GaussDBJsonDbFunctionsExtensions.JsonExists)
                => _sqlExpressionFactory.MakePostgresBinary(GaussDBExpressionType.JsonExists, Jsonb(args[0]), args[1]),
            nameof(GaussDBJsonDbFunctionsExtensions.JsonExistAny)
                => _sqlExpressionFactory.MakePostgresBinary(GaussDBExpressionType.JsonExistsAny, Jsonb(args[0]), args[1]),
            nameof(GaussDBJsonDbFunctionsExtensions.JsonExistAll)
                => _sqlExpressionFactory.MakePostgresBinary(GaussDBExpressionType.JsonExistsAll, Jsonb(args[0]), args[1]),

            _ => null
        };

        SqlExpression Jsonb(SqlExpression e)
            => e.TypeMapping?.StoreType == "jsonb"
                ? e
                : e is SqlConstantExpression or SqlParameterExpression
                    ? _sqlExpressionFactory.ApplyTypeMapping(e, _jsonbTypeMapping)
                    : _sqlExpressionFactory.Convert(e, typeof(string), _jsonbTypeMapping);

        static SqlExpression RemoveConvert(SqlExpression e)
        {
            while (e is SqlUnaryExpression { OperatorType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            {
                e = unary.Operand;
            }

            return e;
        }

        GaussDBJsonTraversalExpression WithReturnsText(GaussDBJsonTraversalExpression traversal, bool returnsText)
            => traversal.ReturnsText == returnsText
                ? traversal
                : returnsText
                    ? new GaussDBJsonTraversalExpression(traversal.Expression, traversal.Path, true, typeof(string), _stringTypeMapping)
                    : new GaussDBJsonTraversalExpression(
                        traversal.Expression, traversal.Path, false, traversal.Type, traversal.Expression.TypeMapping);
    }
}
