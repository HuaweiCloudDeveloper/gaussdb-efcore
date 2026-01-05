using System.Diagnostics.CodeAnalysis;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Extensions.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;
using static HuaweiCloud.EntityFrameworkCore.GaussDB.Utilities.Statics;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
{
    private readonly RelationalQueryCompilationContext _queryCompilationContext;
    private readonly GaussDBTypeMappingSource _typeMappingSource;
    private readonly GaussDBExpressionFactory _sqlExpressionFactory;
    private readonly bool _isRedshift;
    private RelationalTypeMapping? _ordinalityTypeMapping;

    #region MethodInfos

    private static readonly MethodInfo Like2MethodInfo =
        typeof(DbFunctionsExtensions).GetRuntimeMethod(
            nameof(DbFunctionsExtensions.Like), [typeof(DbFunctions), typeof(string), typeof(string)])!;

    // ReSharper disable once InconsistentNaming
    private static readonly MethodInfo ILike2MethodInfo
        = typeof(GaussDBDbFunctionsExtensions).GetRuntimeMethod(
            nameof(GaussDBDbFunctionsExtensions.ILike), [typeof(DbFunctions), typeof(string), typeof(string)])!;

    private static readonly MethodInfo MatchesLQuery =
        typeof(LTree).GetRuntimeMethod(nameof(LTree.MatchesLQuery), [typeof(string)])!;

    #endregion

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        RelationalQueryCompilationContext queryCompilationContext,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
        _queryCompilationContext = queryCompilationContext;
        _typeMappingSource = (GaussDBTypeMappingSource)relationalDependencies.TypeMappingSource;
        _sqlExpressionFactory = (GaussDBExpressionFactory)relationalDependencies.SqlExpressionFactory;
        _isRedshift = GaussDBSingletonOptions.UseRedshift;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBQueryableMethodTranslatingExpressionVisitor(GaussDBQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
        _queryCompilationContext = parentVisitor._queryCompilationContext;
        _typeMappingSource = parentVisitor._typeMappingSource;
        _sqlExpressionFactory = parentVisitor._sqlExpressionFactory;
        _isRedshift = parentVisitor._isRedshift;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new GaussDBQueryableMethodTranslatingExpressionVisitor(this);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression? TranslatePrimitiveCollection(
        SqlExpression sqlExpression,
        IProperty? property,
        string tableAlias)
    {
        if (_isRedshift)
        {
            AddTranslationErrorDetails("Redshift does not support unnest, which is required for most forms of querying of JSON arrays.");

            return null;
        }

        var elementClrType = sqlExpression.Type.GetSequenceType();
        var elementTypeMapping = (RelationalTypeMapping?)sqlExpression.TypeMapping?.ElementTypeMapping;

        // If this is a collection property, get the element's nullability out of metadata. Otherwise, this is a parameter property, in
        // which case we only have the CLR type (note that we cannot produce different SQLs based on the nullability of an *element* in
        // a parameter collection - our caching mechanism only supports varying by the nullability of the parameter itself (i.e. the
        // collection).
        // TODO: if property is non-null, GetElementType() should never be null, but we have #31469 for shadow properties
        var isElementNullable = property?.GetElementType() is null
            ? elementClrType.IsNullableType()
            : property.GetElementType()!.IsNullable;

        // We support two kinds of primitive collections: the standard one with PostgreSQL arrays (where we use the unnest function), and
        // a special case for geometry collections, where we use
        SelectExpression selectExpression;

        // TODO: Parameters have no type mapping. We can check whether the expression type is one of the NTS geometry collection types,
        // though in a perfect world we'd actually infer this. In other words, when the type mapping of the element is inferred further on,
        // we'd replace the unnest expression with ST_Dump. We could even have a special expression type which means "indeterminate, must be
        // inferred".
#pragma warning disable EF1001 // SelectExpression constructors are pubternal
        switch (sqlExpression.TypeMapping)
        {
            case { StoreTypeNameBase: "geometry" or "geography" }:
            {
                // TODO: For geometry collection support (not yet supported), see #2850.
                selectExpression = new SelectExpression(
                    [new TableValuedFunctionExpression(tableAlias, "ST_Dump", [sqlExpression])],
                    new ColumnExpression("geom", tableAlias, elementClrType.UnwrapNullableType(), elementTypeMapping, isElementNullable),
                    identifier: [], // TODO
                    _queryCompilationContext.SqlAliasManager);
                break;
            }

            // Scalar/primitive collection mapped to a PostgreSQL array (typical and default case)
            case GaussDBArrayTypeMapping or GaussDBMultirangeTypeMapping or null:
            {
                // Note that for unnest we have a special expression type extending TableValuedFunctionExpression, adding the ability to provide
                // an explicit column name for its output (SELECT * FROM unnest(array) AS f(foo)).
                // This is necessary since when the column name isn't explicitly specified, it is automatically identical to the table alias
                // (f above); since the table alias may get uniquified by EF, this would break queries.

                // TODO: When we have metadata to determine if the element is nullable, pass that here to SelectExpression

                // Note also that with PostgreSQL unnest, the output ordering is guaranteed to be the same as the input array. However, we still
                // need to add an explicit ordering on the ordinality column, since once the unnest is joined into a select, its "natural"
                // orderings is lost and an explicit ordering is needed again (see #3207).
                var (ordinalityColumn, ordinalityComparer) = GenerateOrdinalityIdentifier(tableAlias);
                selectExpression = new SelectExpression(
                    [new GaussDBUnnestExpression(tableAlias, sqlExpression, "value")],
                    new ColumnExpression(
                        "value",
                        tableAlias,
                        elementClrType.UnwrapNullableType(),
                        elementTypeMapping,
                        isElementNullable),
                    identifier: [(ordinalityColumn, ordinalityComparer)],
                    _queryCompilationContext.SqlAliasManager);

                selectExpression.AppendOrdering(new OrderingExpression(ordinalityColumn, ascending: true));
                break;
            }

            // Scalar/primitive collection mapped to a JSON array, like in other providers.
            // Happens for scalar collections nested within JSON documents, or if the user explicitly mapped to JSON instead of
            // the default GaussDB array.
            // Translate to SELECT element::int FROM jsonb_array_elements_text(...) WITH ORDINALITY
            case GaussDBJsonTypeMapping { ElementTypeMapping: not null, StoreType: var storeType }:
            {
                var (ordinalityColumn, ordinalityComparer) = GenerateOrdinalityIdentifier(tableAlias);

                SqlExpression elementProjection = new ColumnExpression(
                    "element",
                    tableAlias,
                    typeof(string),
                    _typeMappingSource.FindMapping(typeof(string)),
                    isElementNullable);

                // If the projected type is anything other than a text, apply a cast (jsonb_array_elements_text returns text)
                if (!elementTypeMapping!.StoreType.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    elementProjection = _sqlExpressionFactory.Convert(
                        elementProjection,
                        elementClrType.UnwrapNullableType(),
                        elementTypeMapping);
                }

                selectExpression = new SelectExpression(
                    [
                        new GaussDBTableValuedFunctionExpression(
                            tableAlias,
                            storeType switch
                            {
                                "jsonb" => "jsonb_array_elements_text",
                                "json" => "json_array_elements_text",
                                _ => throw new UnreachableException()
                            },
                            [sqlExpression],
                            columnInfos: [new("element")],
                            withOrdinality: true)
                    ],
                    elementProjection,
                    identifier: [(ordinalityColumn, ordinalityComparer)],
                    _queryCompilationContext.SqlAliasManager);

                selectExpression.AppendOrdering(new OrderingExpression(ordinalityColumn, ascending: true));
                break;
            }

            default:
                throw new UnreachableException();
        }
#pragma warning restore EF1001 // SelectExpression constructors are pubternal

        Expression shaperExpression = new ProjectionBindingExpression(
            selectExpression, new ProjectionMember(), elementClrType.MakeNullable());

        if (elementClrType != shaperExpression.Type)
        {
            Check.DebugAssert(
                elementClrType.MakeNullable() == shaperExpression.Type,
                "expression.Type must be nullable of targetType");

            shaperExpression = Expression.Convert(shaperExpression, elementClrType);
        }

        return new ShapedQueryExpression(selectExpression, shaperExpression);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression TransformJsonQueryToTable(JsonQueryExpression jsonQueryExpression)
    {
        // Calculate the table alias for the jsonb_to_recordset function based on the last named path segment
        // (or the JSON column name if there are none)
        var lastNamedPathSegment = jsonQueryExpression.Path.LastOrDefault(ps => ps.PropertyName is not null);
        var tableAlias =
            _queryCompilationContext.SqlAliasManager.GenerateTableAlias(
                lastNamedPathSegment.PropertyName ?? jsonQueryExpression.JsonColumn.Name);

        // TODO: This relies on nested JSON columns flowing across the type mapping of the top-most containing JSON column, check this.
        var functionName = jsonQueryExpression.JsonColumn switch
        {
            { TypeMapping.StoreType: "jsonb" } => "jsonb_to_recordset",
            { TypeMapping.StoreType: "json" } => "json_to_recordset",
            { TypeMapping: null } => throw new UnreachableException("Missing type mapping on JSON column"),

            _ => throw new UnreachableException()
        };

        var jsonTypeMapping = jsonQueryExpression.JsonColumn.TypeMapping!;
        //Check.DebugAssert(jsonTypeMapping is GaussDBStructuralJsonTypeMapping, "JSON column has a non-JSON mapping");

        // We now add all of projected entity's the properties and navigations into the jsonb_to_recordset's AS clause, which defines the
        // names and types of columns to come out of the JSON fragments.
        var columnInfos = new List<GaussDBTableValuedFunctionExpression.ColumnInfo>();

        // We're only interested in properties which actually exist in the JSON, filter out uninteresting shadow keys
        foreach (var property in jsonQueryExpression.StructuralType.GetPropertiesInHierarchy())
        {
            if (property.GetJsonPropertyName() is string jsonPropertyName)
            {
                columnInfos.Add(
                    new GaussDBTableValuedFunctionExpression.ColumnInfo
                    {
                        Name = jsonPropertyName,
                        TypeMapping = property.GetRelationalTypeMapping()
                    });
            }
        }

        switch (jsonQueryExpression.StructuralType)
        {
            case IEntityType entityType:
                foreach (var navigation in entityType.GetNavigationsInHierarchy()
                    .Where(n => n.ForeignKey.IsOwnership
                        && n.TargetEntityType.IsMappedToJson()
                        && n.ForeignKey.PrincipalToDependent == n))
                {
                    var jsonNavigationName = navigation.TargetEntityType.GetJsonPropertyName();
                    Check.DebugAssert(jsonNavigationName is not null, $"No JSON property name for navigation {navigation.Name}");

                    columnInfos.Add(
                        new GaussDBTableValuedFunctionExpression.ColumnInfo { Name = jsonNavigationName, TypeMapping = jsonTypeMapping });
                }

                break;

            case IComplexType complexType:
                foreach (var complexProperty in complexType.GetComplexProperties())
                {
                    var jsonPropertyName = complexProperty.ComplexType.GetJsonPropertyName();
                    Check.DebugAssert(jsonPropertyName is not null, $"No JSON property name for complex property {complexProperty.Name}");

                    columnInfos.Add(
                        new GaussDBTableValuedFunctionExpression.ColumnInfo { Name = jsonPropertyName, TypeMapping = jsonTypeMapping });
                }

                break;

            default:
                throw new UnreachableException();
        }

        // json_to_recordset requires the nested JSON document - it does not accept a path within a containing JSON document (like SQL
        // Server OPENJSON or SQLite json_each). So we wrap json_to_recordset around a JsonScalarExpression which will extract the nested
        // document.
        var jsonScalarExpression = new JsonScalarExpression(
            jsonQueryExpression.JsonColumn, jsonQueryExpression.Path, typeof(string), jsonTypeMapping, jsonQueryExpression.IsNullable);

        // Construct the json_to_recordset around the JsonScalarExpression, and wrap it in a SelectExpression
        var jsonToRecordSetExpression = new GaussDBTableValuedFunctionExpression(
            tableAlias, functionName, [jsonScalarExpression], columnInfos, withOrdinality: true);

#pragma warning disable EF1001 // SelectExpression constructors are currently internal
        var selectExpression = CreateSelect(
            jsonQueryExpression,
            jsonToRecordSetExpression,
            "ordinality",
            typeof(int),
            _typeMappingSource.FindMapping(typeof(int))!);
#pragma warning restore EF1001 // Internal EF Core API usage.

        return new ShapedQueryExpression(
            selectExpression,
            new RelationalStructuralTypeShaperExpression(
                jsonQueryExpression.StructuralType,
                new ProjectionBindingExpression(
                    selectExpression,
                    new ProjectionMember(),
                    typeof(ValueBuffer)),
                false));
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression TranslateConcat(ShapedQueryExpression source1, ShapedQueryExpression source2)
    {
        // Simplify x.Array.Concat(y.Array) => x.Array || y.Array instead of:
        // SELECT u.value FROM unnest(x.Array) UNION ALL SELECT u.value FROM unnest(y.Array)
        if (source1.TryExtractArray(out var array1, out var projectedColumn1)
            && source2.TryExtractArray(out var array2, out var projectedColumn2))
        {
            Check.DebugAssert(projectedColumn1.Type == projectedColumn2.Type, "projectedColumn1.Type == projectedColumn2.Type");
            Check.DebugAssert(
                projectedColumn1.TypeMapping is not null || projectedColumn2.TypeMapping is not null,
                "Concat with no type mapping on either side (operation should be client-evaluated over parameters/constants");

            // TODO: Conflicting type mappings from both sides?
            var inferredTypeMapping = projectedColumn1.TypeMapping ?? projectedColumn2.TypeMapping;

#pragma warning disable EF1001 // SelectExpression constructors are currently internal
            var tableAlias = ((SelectExpression)source1.QueryExpression).Tables.Single().Alias!;
            var selectExpression = new SelectExpression(
                [new GaussDBUnnestExpression(tableAlias, _sqlExpressionFactory.Add(array1, array2), "value")],
                new ColumnExpression("value", tableAlias, projectedColumn1.Type, inferredTypeMapping, projectedColumn1.IsNullable || projectedColumn2.IsNullable),
                identifier: [GenerateOrdinalityIdentifier(tableAlias)],
                _queryCompilationContext.SqlAliasManager);
#pragma warning restore EF1001 // Internal EF Core API usage.

            // TODO: Simplify by using UpdateQueryExpression after https://github.com/dotnet/efcore/issues/31511
            Expression shaperExpression = new ProjectionBindingExpression(
                selectExpression, new ProjectionMember(), source1.ShaperExpression.Type.MakeNullable());

            if (source1.ShaperExpression.Type != shaperExpression.Type)
            {
                Check.DebugAssert(
                    source1.ShaperExpression.Type.MakeNullable() == shaperExpression.Type,
                    "expression.Type must be nullable of targetType");

                shaperExpression = Expression.Convert(shaperExpression, source1.ShaperExpression.Type);
            }

            return new ShapedQueryExpression(selectExpression, shaperExpression);
        }

        return base.TranslateConcat(source1, source2);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression? TranslateSkip(ShapedQueryExpression source, Expression count)
    {
        // Translate Skip over array to the PostgreSQL slice operator (array.Skip(2) -> array[3,])
        // Note that we have unnest over multiranges, not just arrays - but multiranges don't support subscripting/slicing.
        if (source.TryExtractArray(out var array, out var projectedColumn)
            && TranslateExpression(count) is { } translatedCount)
        {
#pragma warning disable EF1001 // SelectExpression constructors are currently internal
            var tableAlias = ((SelectExpression)source.QueryExpression).Tables[0].Alias!;
            var selectExpression = new SelectExpression(
                [
                    new GaussDBUnnestExpression(
                        tableAlias,
                        _sqlExpressionFactory.ArraySlice(
                            array,
                            lowerBound: GenerateOneBasedIndexExpression(translatedCount),
                            upperBound: null,
                            // isColumnNullable: /*projectedColumn.IsNullable*/ true, // TODO: This fails because of a shaper check
                            projectedColumn.IsNullable),
                        "value"),
                ],
                new ColumnExpression("value", tableAlias, projectedColumn.Type, projectedColumn.TypeMapping, projectedColumn.IsNullable),
                identifier: [GenerateOrdinalityIdentifier(tableAlias)],
                _queryCompilationContext.SqlAliasManager);
#pragma warning restore EF1001

            // TODO: Simplify by using UpdateQueryExpression after https://github.com/dotnet/efcore/issues/31511
            Expression shaperExpression = new ProjectionBindingExpression(
                selectExpression, new ProjectionMember(), source.ShaperExpression.Type.MakeNullable());

            if (source.ShaperExpression.Type != shaperExpression.Type)
            {
                Check.DebugAssert(
                    source.ShaperExpression.Type.MakeNullable() == shaperExpression.Type,
                    "expression.Type must be nullable of targetType");

                shaperExpression = Expression.Convert(shaperExpression, source.ShaperExpression.Type);
            }

            return new ShapedQueryExpression(selectExpression, shaperExpression);
        }

        return base.TranslateSkip(source, count);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression? TranslateTake(ShapedQueryExpression source, Expression count)
    {
        // Translate Take over array to the PostgreSQL slice operator (array.Take(2) -> array[,2])
        // Note that we have unnest over multiranges, not just arrays - but multiranges don't support subscripting/slicing.
        if (source.TryExtractArray(out var array, out var projectedColumn)
            && TranslateExpression(count) is { } translatedCount)
        {
            GaussDBArraySliceExpression sliceExpression;

            // If Skip has been called before, an array slice expression is already there; try to integrate this Take into it.
            // Note that we need to take the Skip (lower bound) into account for the Take (upper bound), since the slice upper bound
            // operates on the original array (Skip hasn't yet taken place).
            if (array is GaussDBArraySliceExpression existingSliceExpression)
            {
                if (existingSliceExpression is
                    {
                        LowerBound: SqlConstantExpression { Value: int lowerBoundValue } lowerBound,
                        UpperBound: null
                    })
                {
                    sliceExpression = existingSliceExpression.Update(
                        existingSliceExpression.Array,
                        existingSliceExpression.LowerBound,
                        translatedCount is SqlConstantExpression { Value: int takeCount }
                            ? _sqlExpressionFactory.Constant(lowerBoundValue + takeCount - 1, lowerBound.TypeMapping)
                            : _sqlExpressionFactory.Subtract(
                                _sqlExpressionFactory.Add(lowerBound, translatedCount),
                                _sqlExpressionFactory.Constant(1, lowerBound.TypeMapping)));
                }
                else
                {
                    // For any other case, we allow relational to translate with normal querying. For non-constant lower bounds, we could
                    // duplicate them into the upper bound, but that could cause expensive double evaluation.
                    return base.TranslateTake(source, count);
                }
            }
            else
            {
                sliceExpression = _sqlExpressionFactory.ArraySlice(
                    array,
                    lowerBound: null,
                    upperBound: translatedCount,
                    projectedColumn.IsNullable);
            }

#pragma warning disable EF1001 // SelectExpression constructors are currently internal
            var tableAlias = ((SelectExpression)source.QueryExpression).Tables[0].Alias!;
            var selectExpression = new SelectExpression(
                [new GaussDBUnnestExpression(tableAlias, sliceExpression, "value")],
                new ColumnExpression("value", tableAlias, projectedColumn.Type, projectedColumn.TypeMapping, projectedColumn.IsNullable),
                [GenerateOrdinalityIdentifier(tableAlias)],
                _queryCompilationContext.SqlAliasManager);
#pragma warning restore EF1001 // Internal EF Core API usage.

            // TODO: Simplify by using UpdateQueryExpression after https://github.com/dotnet/efcore/issues/31511
            Expression shaperExpression = new ProjectionBindingExpression(
                selectExpression, new ProjectionMember(), source.ShaperExpression.Type.MakeNullable());

            if (source.ShaperExpression.Type != shaperExpression.Type)
            {
                Check.DebugAssert(
                    source.ShaperExpression.Type.MakeNullable() == shaperExpression.Type,
                    "expression.Type must be nullable of targetType");

                shaperExpression = Expression.Convert(shaperExpression, source.ShaperExpression.Type);
            }

            return new ShapedQueryExpression(selectExpression, shaperExpression);
        }

        return base.TranslateTake(source, count);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ShapedQueryExpression? TranslateWhere(ShapedQueryExpression source, LambdaExpression predicate)
    {
        // Simplify x.Array.Where(i => i != 3) => array_remove(x.Array, 3) instead of subquery
        if (predicate.Body is BinaryExpression
            {
                NodeType: ExpressionType.NotEqual,
                Left: var left,
                Right: var right
            }
            && (left == predicate.Parameters[0] ? right : right == predicate.Parameters[0] ? left : null) is Expression itemToFilterOut
            && source.TryExtractArray(out var array, out var projectedColumn)
            && TranslateExpression(itemToFilterOut) is SqlExpression translatedItemToFilterOut)
        {
            var simplifiedTranslation = _sqlExpressionFactory.Function(
                "array_remove",
                [array, translatedItemToFilterOut],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[2],
                array.Type,
                array.TypeMapping);

#pragma warning disable EF1001 // SelectExpression constructors are currently internal
            var tableAlias = ((SelectExpression)source.QueryExpression).Tables[0].Alias!;
            var selectExpression = new SelectExpression(
                [new GaussDBUnnestExpression(tableAlias, simplifiedTranslation, "value")],
                new ColumnExpression("value", tableAlias, projectedColumn.Type, projectedColumn.TypeMapping, projectedColumn.IsNullable),
                [GenerateOrdinalityIdentifier(tableAlias)],
                _queryCompilationContext.SqlAliasManager);
#pragma warning restore EF1001 // Internal EF Core API usage.

            // TODO: Simplify by using UpdateQueryExpression after https://github.com/dotnet/efcore/issues/31511
            Expression shaperExpression = new ProjectionBindingExpression(
                selectExpression, new ProjectionMember(), source.ShaperExpression.Type.MakeNullable());

            if (source.ShaperExpression.Type != shaperExpression.Type)
            {
                Check.DebugAssert(
                    source.ShaperExpression.Type.MakeNullable() == shaperExpression.Type,
                    "expression.Type must be nullable of targetType");

                shaperExpression = Expression.Convert(shaperExpression, source.ShaperExpression.Type);
            }

            return new ShapedQueryExpression(selectExpression, shaperExpression);
        }

        return base.TranslateWhere(source, predicate);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsNaturallyOrdered(SelectExpression selectExpression)
        => selectExpression is { Tables: [GaussDBUnnestExpression unnest, ..] }
            && (selectExpression.Orderings is []
                || selectExpression.Orderings is
                    [{ Expression: ColumnExpression { Name: "ordinality", TableAlias: var orderingTableAlias } }]
                && orderingTableAlias == unnest.Alias);

    #region ExecuteUpdate

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsValidSelectExpressionForExecuteUpdate(
        SelectExpression selectExpression,
        TableExpressionBase targetTable,
        [NotNullWhen(true)] out TableExpression? tableExpression)
    {
        if (!base.IsValidSelectExpressionForExecuteUpdate(selectExpression, targetTable, out tableExpression))
        {
            return false;
        }

        // PostgreSQL doesn't support referencing the main update table from anywhere except for the UPDATE WHERE clause.
        // This specifically makes it impossible to have joins which reference the main table in their predicate (ON ...).
        // Because of this, we detect all such inner joins and lift their predicates to the main WHERE clause (where a reference to the
        // main table is allowed) - see GaussDBQuerySqlGenerator.VisitUpdate.
        // For any other type of join which contains a reference to the main table, we return false to trigger a subquery pushdown instead.
        OuterReferenceFindingExpressionVisitor? visitor = null;

        for (var i = 0; i < selectExpression.Tables.Count; i++)
        {
            var table = selectExpression.Tables[i];

            if (ReferenceEquals(table, tableExpression))
            {
                continue;
            }

            visitor ??= new OuterReferenceFindingExpressionVisitor(tableExpression);

            // For inner joins, if the predicate contains a reference to the main table, GaussDBQuerySqlGenerator will lift the predicate
            // to the WHERE clause; so we only need to check the inner join's table (i.e. subquery) for such a reference.
            // Cross join and cross/outer apply (lateral joins) don't have predicates, so just check the entire join for a reference to
            // the main table, and switch to subquery syntax if one is found.
            // Left join does have a predicate, but it isn't possible to lift it to the main WHERE clause; so also check the entire
            // join.
            if (table is InnerJoinExpression innerJoin)
            {
                table = innerJoin.Table;
            }

            if (visitor.ContainsReferenceToMainTable(table))
            {
                return false;
            }
        }

        return true;
    }

#pragma warning disable EF9002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool TrySerializeScalarToJson(
        JsonScalarExpression target,
        SqlExpression value,
        [NotNullWhen(true)] out SqlExpression? jsonValue)
    {
        var jsonTypeMapping = ((ColumnExpression)target.Json).TypeMapping!;

        if (
            // The base implementation doesn't handle serializing arbitrary SQL expressions to JSON, since that's
            // database-specific. In PostgreSQL we simply do this by wrapping any expression in to_jsonb().
            !base.TrySerializeScalarToJson(target, value, out jsonValue)
            // In addition, for string, numeric and bool, the base implementation simply returns the value as-is, since most databases allow
            // passing these native types directly to their JSON partial update function. In PostgreSQL, jsonb_set() always requires jsonb,
            // so we wrap those expression with to_jsonb() as well.
            || jsonValue.TypeMapping?.StoreType is not "jsonb" and not "json")
        {
            switch (value.TypeMapping!.StoreType)
            {
                case "jsonb" or "json":
                    jsonValue = value;
                    return true;

                case "bytea":
                    value = _sqlExpressionFactory.Function(
                        "encode",
                        [value, _sqlExpressionFactory.Constant("base64")],
                        nullable: true,
                        argumentsPropagateNullability: [true, true],
                        typeof(string),
                        _typeMappingSource.FindMapping(typeof(string))!
                    );
                    break;
            }

            // We now have a scalar value expression that needs to be passed to jsonb_set(), but jsonb_set() requires a json/jsonb
            // argument, not e.g. text or int. So we need to wrap the argument in to_jsonb/to_json.
            // Note that for structural types we always already get a jsonb/json value and have already exited above (no need for
            // to_jsonb/to_json).

            // One exception is if the value expression happens to be a JsonScalarExpression (e.g. copy scalar property from within
            // one JSON document into another). For that case, rather than do to_jsonb(x.JsonbDoc ->> 'SomeProperty') - which extracts
            // a jsonb property as text only to reconvert it back to jsonb - we just change the type mapping on the JsonScalarExpression
            // to json/jsonb, in order to generate x.JsonbDoc -> 'SomeProperty' (no text extraction).
            if (value is JsonScalarExpression jsonScalarValue
                && jsonScalarValue.Json.TypeMapping?.StoreType == jsonTypeMapping.StoreType)
            {
                jsonValue = new JsonScalarExpression(
                    jsonScalarValue.Json,
                    jsonScalarValue.Path,
                    jsonScalarValue.Type,
                    jsonTypeMapping,
                    jsonScalarValue.IsNullable);
                return true;
            }

            jsonValue = _sqlExpressionFactory.Function(
                jsonTypeMapping.StoreType switch
                {
                    "jsonb" => "to_jsonb",
                    "json" => "to_json",
                    _ => throw new UnreachableException()
                },
                // Make sure GaussDB interprets constant values correctly by adding explicit typing based on the target property's type mapping.
                // Note that we can only be here for scalar properties, for structural types we always already get a jsonb/json value
                // and don't need to add to_jsonb/to_json.
                [value is SqlConstantExpression ? _sqlExpressionFactory.Convert(value, target.Type, target.TypeMapping) : value],
                nullable: true,
                argumentsPropagateNullability: [true],
                typeof(string),
                jsonTypeMapping);
        }

        return true;
    }
#pragma warning restore EF9002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override SqlExpression? GenerateJsonPartialUpdateSetter(
        Expression target,
        SqlExpression value,
        ref SqlExpression? existingSetterValue)
    {
        var (jsonColumn, path) = target switch
        {
            JsonScalarExpression j => ((ColumnExpression)j.Json, j.Path),
            JsonQueryExpression j => (j.JsonColumn, j.Path),

            _ => throw new UnreachableException(),
        };

        var jsonSet = _sqlExpressionFactory.Function(
            jsonColumn.TypeMapping?.StoreType switch
            {
                "jsonb" => "jsonb_set",
                "json" => "json_set",
                _ => throw new UnreachableException()
            },
            arguments:
            [
                existingSetterValue ?? jsonColumn,
                // Hack: Rendering of JSONPATH strings happens in value generation. We can have a special expression for modify to hold the
                // IReadOnlyList<PathSegment> (just like Json{Scalar,Query}Expression), but instead we do the slight hack of packaging it
                // as a constant argument; it will be unpacked and handled in SQL generation.
                _sqlExpressionFactory.Constant(path, RelationalTypeMapping.NullMapping),
                value
            ],
            nullable: true,
            argumentsPropagateNullability: [true, true, true],
            typeof(string),
            jsonColumn.TypeMapping);

        if (existingSetterValue is null)
        {
            return jsonSet;
        }
        else
        {
            existingSetterValue = jsonSet;
            return null;
        }
    }

    #endregion ExecuteUpdate

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsValidSelectExpressionForExecuteDelete(SelectExpression selectExpression)
        // The default relational behavior is to allow only single-table expressions, and the only permitted feature is a predicate.
        // Here we extend this to also inner joins to tables, which we generate via the PostgreSQL-specific USING construct.
        => selectExpression is
        {
            Orderings: [],
            Offset: null,
            Limit: null,
            GroupBy: [],
            Having: null
        }
        && selectExpression.Tables[0] is TableExpression && selectExpression.Tables.Skip(1).All(t => t is InnerJoinExpression);

    // PostgreSQL unnest is guaranteed to return output rows in the same order as its input array,
    // https://www.postgresql.org/docs/current/functions-array.html.
    /// <inheritdoc />
    protected override bool IsOrdered(SelectExpression selectExpression)
        => base.IsOrdered(selectExpression)
            || selectExpression.Tables is
                [GaussDBTableValuedFunctionExpression { Name: "unnest" or "jsonb_to_recordset" or "json_to_recordset" }];

    private (ColumnExpression, ValueComparer) GenerateOrdinalityIdentifier(string tableAlias)
    {
        _ordinalityTypeMapping ??= _typeMappingSource.FindMapping("int")!;
        return (new ColumnExpression("ordinality", tableAlias, typeof(int), _ordinalityTypeMapping, nullable: false),
            _ordinalityTypeMapping.Comparer);
    }

    /// <summary>
    ///     PostgreSQL array indexing is 1-based. If the index happens to be a constant, just increment it. Otherwise, append a +1 in the
    ///     SQL.
    /// </summary>
    private SqlExpression GenerateOneBasedIndexExpression(SqlExpression expression)
        => expression is SqlConstantExpression constant
            ? _sqlExpressionFactory.Constant(Convert.ToInt32(constant.Value) + 1, constant.TypeMapping)
            : _sqlExpressionFactory.Add(expression, _sqlExpressionFactory.Constant(1));

#pragma warning disable EF1001 // SelectExpression constructors are currently internal
    private ShapedQueryExpression BuildSimplifiedShapedQuery(ShapedQueryExpression source, SqlExpression translation)
        => source.Update(
            new SelectExpression(translation, _queryCompilationContext.SqlAliasManager),
            Expression.Convert(
                new ProjectionBindingExpression(translation, new ProjectionMember(), typeof(bool?)), typeof(bool)));
#pragma warning restore EF1001

    private sealed class OuterReferenceFindingExpressionVisitor(TableExpression mainTable) : ExpressionVisitor
    {
        private bool _containsReference;

        public bool ContainsReferenceToMainTable(TableExpressionBase tableExpression)
        {
            _containsReference = false;

            Visit(tableExpression);

            return _containsReference;
        }

        [return: NotNullIfNotNull("expression")]
        public override Expression? Visit(Expression? expression)
        {
            if (_containsReference)
            {
                return expression;
            }

            if (expression is ColumnExpression { TableAlias: var tableAlias } && tableAlias == mainTable.Alias)
            {
                _containsReference = true;

                return expression;
            }

            return base.Visit(expression);
        }
    }
}
