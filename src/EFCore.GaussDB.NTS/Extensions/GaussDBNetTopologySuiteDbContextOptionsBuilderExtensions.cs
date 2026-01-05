using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     NetTopologySuite specific extension methods for <see cref="GaussDBDbContextOptionsBuilder" />.
/// </summary>
public static class GaussDBNetTopologySuiteDbContextOptionsBuilderExtensions
{
    /// <summary>
    ///     Use NetTopologySuite to access SQL Server spatial data.
    /// </summary>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static GaussDBDbContextOptionsBuilder UseNetTopologySuite(
        this GaussDBDbContextOptionsBuilder optionsBuilder,
        CoordinateSequenceFactory? coordinateSequenceFactory = null,
        PrecisionModel? precisionModel = null,
        Ordinates handleOrdinates = Ordinates.None,
        bool geographyAsDefault = false)
    {
        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;

        var extension = coreOptionsBuilder.Options.FindExtension<GaussDBNetTopologySuiteOptionsExtension>()
            ?? new GaussDBNetTopologySuiteOptionsExtension();

        if (coordinateSequenceFactory is not null)
        {
            extension = extension.WithCoordinateSequenceFactory(coordinateSequenceFactory);
        }

        if (precisionModel is not null)
        {
            extension = extension.WithPrecisionModel(precisionModel);
        }

        if (handleOrdinates is not Ordinates.None)
        {
            extension = extension.WithHandleOrdinates(handleOrdinates);
        }

        if (geographyAsDefault)
        {
            extension = extension.WithGeographyDefault();
        }

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}
