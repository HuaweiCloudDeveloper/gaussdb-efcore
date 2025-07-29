using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;

/// <inheritdoc />
public class NpgsqlNetTopologySuiteSingletonOptions : INpgsqlNetTopologySuiteSingletonOptions
{
    /// <inheritdoc />
    public virtual CoordinateSequenceFactory? CoordinateSequenceFactory { get; set; }

    /// <inheritdoc />
    public virtual PrecisionModel? PrecisionModel { get; set; }

    /// <inheritdoc />
    public virtual Ordinates HandleOrdinates { get; set; }

    /// <inheritdoc />
    public virtual bool IsGeographyDefault { get; set; }

    /// <inheritdoc />
    public virtual void Initialize(IDbContextOptions options)
    {
        var npgsqlNtsOptions = options.FindExtension<NpgsqlNetTopologySuiteOptionsExtension>()
            ?? new NpgsqlNetTopologySuiteOptionsExtension();

        CoordinateSequenceFactory = npgsqlNtsOptions.CoordinateSequenceFactory;
        PrecisionModel = npgsqlNtsOptions.PrecisionModel;
        HandleOrdinates = npgsqlNtsOptions.HandleOrdinates;
        IsGeographyDefault = npgsqlNtsOptions.IsGeographyDefault;
    }

    /// <inheritdoc />
    public virtual void Validate(IDbContextOptions options) { }
}
