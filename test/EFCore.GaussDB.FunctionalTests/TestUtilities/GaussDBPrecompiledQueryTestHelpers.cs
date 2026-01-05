using Microsoft.CodeAnalysis;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBPrecompiledQueryTestHelpers : PrecompiledQueryTestHelpers
{
    public static GaussDBPrecompiledQueryTestHelpers Instance = new();

    protected override IEnumerable<MetadataReference> BuildProviderMetadataReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(GaussDBOptionsExtension).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(GaussDBConnection).Assembly.Location);
        yield return MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location);
    }
}
