using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.NetTopologySuite.Scaffolding.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBNetTopologySuiteCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
{
    private static readonly MethodInfo _useNetTopologySuiteMethodInfo
        = typeof(GaussDBNetTopologySuiteDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite),
            typeof(GaussDBDbContextOptionsBuilder),
            typeof(CoordinateSequenceFactory),
            typeof(PrecisionModel),
            typeof(Ordinates),
            typeof(bool));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override MethodCallCodeFragment GenerateProviderOptions()
        => new(_useNetTopologySuiteMethodInfo);
}
