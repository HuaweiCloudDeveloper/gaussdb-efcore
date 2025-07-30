using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Extension methods for <see cref="AlterDatabaseOperation" /> for GaussDB-specific metadata.
/// </summary>
public static class GaussDBAlterDatabaseOperationExtensions
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBCollation> GetPostgresCollations(this AlterDatabaseOperation operation)
        => GaussDBCollation.GetCollations(operation).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBCollation> GetOldPostgresCollations(this AlterDatabaseOperation operation)
        => GaussDBCollation.GetCollations(operation.OldDatabase).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBExtension> GetPostgresExtensions(this AlterDatabaseOperation operation)
        => GaussDBExtension.GetPostgresExtensions(operation).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBExtension> GetOldPostgresExtensions(this AlterDatabaseOperation operation)
        => GaussDBExtension.GetPostgresExtensions(operation.OldDatabase).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBEnum> GetPostgresEnums(this AlterDatabaseOperation operation)
        => GaussDBEnum.GetPostgresEnums(operation).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<GaussDBEnum> GetOldPostgresEnums(this AlterDatabaseOperation operation)
        => GaussDBEnum.GetPostgresEnums(operation.OldDatabase).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.GaussDBRange> GetPostgresRanges(this AlterDatabaseOperation operation)
        => HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.GaussDBRange.GetPostgresRanges(operation).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IReadOnlyList<HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.GaussDBRange> GetOldPostgresRanges(this AlterDatabaseOperation operation)
        => HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.GaussDBRange.GetPostgresRanges(operation.OldDatabase).ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static GaussDBExtension GetOrAddPostgresExtension(
        this AlterDatabaseOperation operation,
        string? schema,
        string name,
        string? version)
        => GaussDBExtension.GetOrAddPostgresExtension(operation, schema, name, version);
}
