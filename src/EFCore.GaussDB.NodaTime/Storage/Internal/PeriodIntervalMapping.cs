using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;
using NodaTime.Text;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class PeriodIntervalMapping : GaussDBTypeMapping
{
    private static readonly MethodInfo FromYears = typeof(Period).GetRuntimeMethod(nameof(Period.FromYears), [typeof(int)])!;
    private static readonly MethodInfo FromMonths = typeof(Period).GetRuntimeMethod(nameof(Period.FromMonths), [typeof(int)])!;
    private static readonly MethodInfo FromWeeks = typeof(Period).GetRuntimeMethod(nameof(Period.FromWeeks), [typeof(int)])!;
    private static readonly MethodInfo FromDays = typeof(Period).GetRuntimeMethod(nameof(Period.FromDays), [typeof(int)])!;
    private static readonly MethodInfo FromHours = typeof(Period).GetRuntimeMethod(nameof(Period.FromHours), [typeof(long)])!;
    private static readonly MethodInfo FromMinutes = typeof(Period).GetRuntimeMethod(nameof(Period.FromMinutes), [typeof(long)])!;
    private static readonly MethodInfo FromSeconds = typeof(Period).GetRuntimeMethod(nameof(Period.FromSeconds), [typeof(long)])!;

    private static readonly MethodInfo FromMilliseconds = typeof(Period).GetRuntimeMethod(
        nameof(Period.FromMilliseconds), [typeof(long)])!;

    private static readonly MethodInfo FromNanoseconds = typeof(Period).GetRuntimeMethod(
        nameof(Period.FromNanoseconds), [typeof(long)])!;

    private static readonly PropertyInfo Zero = typeof(Period).GetProperty(nameof(Period.Zero))!;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static PeriodIntervalMapping Default { get; } = new();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public PeriodIntervalMapping()
        : base("interval", typeof(Period), GaussDBDbType.Interval, JsonPeriodReaderWriter.Instance)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected PeriodIntervalMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, GaussDBDbType.Interval)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new PeriodIntervalMapping(parameters);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override RelationalTypeMapping WithStoreTypeAndSize(string storeType, int? size)
        => new PeriodIntervalMapping(Parameters.WithStoreTypeAndSize(storeType, size));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"INTERVAL '{GenerateLiteralCore(value)}'";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateEmbeddedNonNullSqlLiteral(object value)
        => $"""
            "{GenerateLiteralCore(value)}"
            """;

    private string GenerateLiteralCore(object value)
        => PeriodPattern.NormalizingIso.Format((Period)value);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression GenerateCodeLiteral(object value)
    {
        var period = (Period)value;
        Expression? e = null;

        if (period.Years != 0)
        {
            Compose(Expression.Call(FromYears, Expression.Constant(period.Years)));
        }

        if (period.Months != 0)
        {
            Compose(Expression.Call(FromMonths, Expression.Constant(period.Months)));
        }

        if (period.Weeks != 0)
        {
            Compose(Expression.Call(FromWeeks, Expression.Constant(period.Weeks)));
        }

        if (period.Days != 0)
        {
            Compose(Expression.Call(FromDays, Expression.Constant(period.Days)));
        }

        if (period.Hours != 0)
        {
            Compose(Expression.Call(FromHours, Expression.Constant(period.Hours)));
        }

        if (period.Minutes != 0)
        {
            Compose(Expression.Call(FromMinutes, Expression.Constant(period.Minutes)));
        }

        if (period.Seconds != 0)
        {
            Compose(Expression.Call(FromSeconds, Expression.Constant(period.Seconds)));
        }

        if (period.Milliseconds != 0)
        {
            Compose(Expression.Call(FromMilliseconds, Expression.Constant(period.Milliseconds)));
        }

        if (period.Nanoseconds != 0)
        {
            Compose(Expression.Call(FromNanoseconds, Expression.Constant(period.Nanoseconds)));
        }

        return e ?? Expression.MakeMemberAccess(null, Zero);

        void Compose(Expression toAdd)
            => e = e is null ? toAdd : Expression.Add(e, toAdd);
    }

    private sealed class JsonPeriodReaderWriter : JsonValueReaderWriter<Period>
    {
        private static readonly PropertyInfo InstanceProperty = typeof(JsonPeriodReaderWriter).GetProperty(nameof(Instance))!;

        public static JsonPeriodReaderWriter Instance { get; } = new();

        public override Period FromJsonTyped(ref Utf8JsonReaderManager manager, object? existingObject = null)
            => PeriodPattern.NormalizingIso.Parse(manager.CurrentReader.GetString()!).GetValueOrThrow();

        public override void ToJsonTyped(Utf8JsonWriter writer, Period value)
            => writer.WriteStringValue(PeriodPattern.NormalizingIso.Format(value));

        /// <inheritdoc />
        public override Expression ConstructorExpression => Expression.Property(null, InstanceProperty);
    }
}
