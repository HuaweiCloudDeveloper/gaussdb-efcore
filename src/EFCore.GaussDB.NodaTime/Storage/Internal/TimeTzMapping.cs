using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;
using NodaTime.Text;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;
using static HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime.Utilties.Util;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class TimeTzMapping : GaussDBTypeMapping
{
    private static readonly ConstructorInfo OffsetTimeConstructor =
        typeof(OffsetTime).GetConstructor([typeof(LocalTime), typeof(Offset)])!;

    private static readonly ConstructorInfo LocalTimeConstructorWithMinutes =
        typeof(LocalTime).GetConstructor([typeof(int), typeof(int)])!;

    private static readonly ConstructorInfo LocalTimeConstructorWithSeconds =
        typeof(LocalTime).GetConstructor([typeof(int), typeof(int), typeof(int)])!;

    private static readonly MethodInfo LocalTimeFromHourMinuteSecondNanosecondMethod =
        typeof(LocalTime).GetMethod(
            nameof(LocalTime.FromHourMinuteSecondNanosecond),
            [typeof(int), typeof(int), typeof(int), typeof(long)])!;

    private static readonly MethodInfo OffsetFromHoursMethod =
        typeof(Offset).GetMethod(nameof(Offset.FromHours), [typeof(int)])!;

    private static readonly MethodInfo OffsetFromSeconds =
        typeof(Offset).GetMethod(nameof(Offset.FromSeconds), [typeof(int)])!;

    private static readonly OffsetTimePattern Pattern =
        OffsetTimePattern.CreateWithInvariantCulture("HH':'mm':'ss;FFFFFFo<G>");

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static TimeTzMapping Default { get; } = new();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public TimeTzMapping()
        : base("time with time zone", typeof(OffsetTime), GaussDBDbType.TimeTz, JsonOffsetTimeReaderWriter.Instance)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected TimeTzMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, GaussDBDbType.TimeTz)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new TimeTzMapping(parameters);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override RelationalTypeMapping WithStoreTypeAndSize(string storeType, int? size)
        => new TimeTzMapping(Parameters.WithStoreTypeAndSize(storeType, size));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string ProcessStoreType(RelationalTypeMappingParameters parameters, string storeType, string _)
        => parameters.Precision is null ? storeType : $"timestamp({parameters.Precision}) with time zone";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"TIMETZ '{Pattern.Format((OffsetTime)value)}'";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateEmbeddedNonNullSqlLiteral(object value)
        => $"""
            "{Pattern.Format((OffsetTime)value)}"
            """;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression GenerateCodeLiteral(object value)
    {
        var offsetTime = (OffsetTime)value;
        var offsetSeconds = offsetTime.Offset.Seconds;

        Expression newLocalTimeExpr;
        if (offsetTime.NanosecondOfSecond != 0)
        {
            newLocalTimeExpr = ConstantCall(
                LocalTimeFromHourMinuteSecondNanosecondMethod, offsetTime.Hour, offsetTime.Minute, offsetTime.Second,
                (long)offsetTime.NanosecondOfSecond);
        }
        else if (offsetTime.Second != 0)
        {
            newLocalTimeExpr = ConstantNew(LocalTimeConstructorWithSeconds, offsetTime.Hour, offsetTime.Minute, offsetTime.Second);
        }
        else
        {
            newLocalTimeExpr = ConstantNew(LocalTimeConstructorWithMinutes, offsetTime.Hour, offsetTime.Minute);
        }

        return Expression.New(
            OffsetTimeConstructor,
            newLocalTimeExpr,
            offsetSeconds % 3600 == 0
                ? ConstantCall(OffsetFromHoursMethod, offsetSeconds / 3600)
                : ConstantCall(OffsetFromSeconds, offsetSeconds));
    }

    private sealed class JsonOffsetTimeReaderWriter : JsonValueReaderWriter<OffsetTime>
    {
        private static readonly PropertyInfo InstanceProperty = typeof(JsonOffsetTimeReaderWriter).GetProperty(nameof(Instance))!;

        public static JsonOffsetTimeReaderWriter Instance { get; } = new();

        public override OffsetTime FromJsonTyped(ref Utf8JsonReaderManager manager, object? existingObject = null)
            => Pattern.Parse(manager.CurrentReader.GetString()!).GetValueOrThrow();

        public override void ToJsonTyped(Utf8JsonWriter writer, OffsetTime value)
            => writer.WriteStringValue(Pattern.Format(value));

        /// <inheritdoc />
        public override Expression ConstructorExpression => Expression.Property(null, InstanceProperty);
    }
}
