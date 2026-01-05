using System.Data.Common;
using System.Text;
using System.Text.Json;
using HuaweiCloud.GaussDB;
using HuaweiCloud.GaussDBTypes;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

/// <summary>
<<<<<<<< HEAD:src/EFCore.GaussDB/Storage/Internal/Mapping/NpgsqlStructuralJsonTypeMapping.cs
///     Supports the standard EF JSON support, which relies on owned entity or complex type modeling.
///     See <see cref="NpgsqlJsonTypeMapping" /> for the older Npgsql-specific support, which allows mapping json/jsonb to text, to e.g.
///     <see cref="JsonElement" /> (weakly-typed mapping) or to arbitrary POCOs (but without them being modeled).
/// </summary>
public class NpgsqlStructuralJsonTypeMapping : JsonTypeMapping
========
///     Supports the standard EF JSON support, which relies on owned entity modeling.
///     See <see cref="GaussDBJsonTypeMapping" /> for the older GaussDB-specific support, which allows mapping json/jsonb to text, to e.g.
///     <see cref="JsonElement" /> (weakly-typed mapping) or to arbitrary POCOs (but without them being modeled).
/// </summary>
public class GaussDBOwnedJsonTypeMapping : JsonTypeMapping
>>>>>>>> develop:src/EFCore.GaussDB/Storage/Internal/Mapping/GaussDBOwnedJsonTypeMapping.cs
{
    /// <summary>
    ///     The database type used by GaussDB (<see cref="GaussDBDbType.Json" /> or <see cref="GaussDBDbType.Jsonb" />.
    /// </summary>
    public virtual GaussDBDbType GaussDBDbType { get; }

    private static readonly MethodInfo GetStringMethod
        = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), [typeof(int)])!;

    private static readonly PropertyInfo UTF8Property
        = typeof(Encoding).GetProperty(nameof(Encoding.UTF8))!;

    private static readonly MethodInfo EncodingGetBytesMethod
        = typeof(Encoding).GetMethod(nameof(Encoding.GetBytes), [typeof(string)])!;

    private static readonly ConstructorInfo MemoryStreamConstructor
        = typeof(MemoryStream).GetConstructor([typeof(byte[])])!;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
<<<<<<<< HEAD:src/EFCore.GaussDB/Storage/Internal/Mapping/NpgsqlStructuralJsonTypeMapping.cs
    public NpgsqlStructuralJsonTypeMapping(string storeType)
        : base(storeType, typeof(JsonTypePlaceholder), dbType: null)
========
    public GaussDBOwnedJsonTypeMapping(string storeType)
        : base(storeType, typeof(JsonElement), dbType: null)
>>>>>>>> develop:src/EFCore.GaussDB/Storage/Internal/Mapping/GaussDBOwnedJsonTypeMapping.cs
    {
        GaussDBDbType = storeType switch
        {
            "json" => GaussDBDbType.Json,
            "jsonb" => GaussDBDbType.Jsonb,
            _ => throw new ArgumentException("Only the json and jsonb types are supported", nameof(storeType))
        };
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override MethodInfo GetDataReaderMethod()
        => GetStringMethod;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression CustomizeDataReaderExpression(Expression expression)
        => Expression.New(
            MemoryStreamConstructor,
            Expression.Call(
                Expression.Property(null, UTF8Property),
                EncodingGetBytesMethod,
                expression));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
<<<<<<<< HEAD:src/EFCore.GaussDB/Storage/Internal/Mapping/NpgsqlStructuralJsonTypeMapping.cs
    protected NpgsqlStructuralJsonTypeMapping(RelationalTypeMappingParameters parameters, NpgsqlDbType npgsqlDbType)
========
    protected GaussDBOwnedJsonTypeMapping(RelationalTypeMappingParameters parameters, GaussDBDbType npgsqlDbType)
>>>>>>>> develop:src/EFCore.GaussDB/Storage/Internal/Mapping/GaussDBOwnedJsonTypeMapping.cs
        : base(parameters)
    {
        GaussDBDbType = npgsqlDbType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override void ConfigureParameter(DbParameter parameter)
    {
        if (parameter is not GaussDBParameter npgsqlParameter)
        {
            throw new InvalidOperationException(
<<<<<<<< HEAD:src/EFCore.GaussDB/Storage/Internal/Mapping/NpgsqlStructuralJsonTypeMapping.cs
                $"Npgsql-specific type mapping {nameof(NpgsqlStructuralJsonTypeMapping)} being used with non-Npgsql parameter type {parameter.GetType().Name}");
========
                $"GaussDB-specific type mapping {nameof(GaussDBOwnedJsonTypeMapping)} being used with non-GaussDB parameter type {parameter.GetType().Name}");
>>>>>>>> develop:src/EFCore.GaussDB/Storage/Internal/Mapping/GaussDBOwnedJsonTypeMapping.cs
        }

        base.ConfigureParameter(parameter);
        npgsqlParameter.GaussDBDbType = GaussDBDbType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual string EscapeSqlLiteral(string literal)
        => literal.Replace("'", "''");

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"'{EscapeSqlLiteral((string)value)}'";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
<<<<<<<< HEAD:src/EFCore.GaussDB/Storage/Internal/Mapping/NpgsqlStructuralJsonTypeMapping.cs
        => new NpgsqlStructuralJsonTypeMapping(parameters, NpgsqlDbType);
========
        => new GaussDBOwnedJsonTypeMapping(parameters, GaussDBDbType);
>>>>>>>> develop:src/EFCore.GaussDB/Storage/Internal/Mapping/GaussDBOwnedJsonTypeMapping.cs
}
