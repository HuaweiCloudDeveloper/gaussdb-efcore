

using System.Text;
using NodaTime.Text;
// ReSharper disable once CheckNamespace
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class IntervalRangeMapping : GaussDBTypeMapping
{
    private static readonly ConstructorInfo _constructor =
        typeof(Interval).GetConstructor([typeof(Instant), typeof(Instant)])!;

    private static readonly ConstructorInfo _constructorWithNulls =
        typeof(Interval).GetConstructor([typeof(Instant?), typeof(Instant?)])!;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IntervalRangeMapping Default { get; } = new();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public IntervalRangeMapping()
        : base("tstzrange", typeof(Interval), GaussDBDbType.TimestampTzRange)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected IntervalRangeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, GaussDBDbType.TimestampTzRange)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new IntervalRangeMapping(parameters);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override RelationalTypeMapping WithStoreTypeAndSize(string storeType, int? size)
        => new IntervalRangeMapping(Parameters.WithStoreTypeAndSize(storeType, size));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"'{GenerateEmbeddedNonNullSqlLiteral(value)}'::tstzrange";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateEmbeddedNonNullSqlLiteral(object value)
    {
        var interval = (Interval)value;

        var stringBuilder = new StringBuilder("[");

        if (interval.HasStart)
        {
            stringBuilder.Append(InstantPattern.ExtendedIso.Format(interval.Start));
        }

        stringBuilder.Append(',');

        if (interval.HasEnd)
        {
            stringBuilder.Append(InstantPattern.ExtendedIso.Format(interval.End));
        }

        stringBuilder.Append(')');

        return stringBuilder.ToString();
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression GenerateCodeLiteral(object value)
    {
        var interval = (Interval)value;

        return interval is { HasStart: true, HasEnd: true }
            ? Expression.New(
                _constructor,
                TimestampTzInstantMapping.GenerateCodeLiteral(interval.Start),
                TimestampTzInstantMapping.GenerateCodeLiteral(interval.End))
            : Expression.New(
                _constructorWithNulls,
                interval.HasStart
                    ? Expression.Convert(
                        TimestampTzInstantMapping.GenerateCodeLiteral(interval.Start),
                        typeof(Instant?))
                    : Expression.Constant(null, typeof(Instant?)),
                interval.HasEnd
                    ? Expression.Convert(
                        TimestampTzInstantMapping.GenerateCodeLiteral(interval.End),
                        typeof(Instant?))
                    : Expression.Constant(null, typeof(Instant?)));
    }
}
