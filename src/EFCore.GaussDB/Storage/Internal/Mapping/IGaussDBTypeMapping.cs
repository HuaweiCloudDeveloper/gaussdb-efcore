namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public interface IGaussDBTypeMapping
{
    /// <summary>
    ///     The database type used by GaussDB.
    /// </summary>
    GaussDBDbType GaussDBDbType { get; }
}
