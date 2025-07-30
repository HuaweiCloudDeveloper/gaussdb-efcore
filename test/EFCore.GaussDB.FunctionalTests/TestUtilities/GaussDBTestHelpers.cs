using HuaweiCloud.EntityFrameworkCore.GaussDB.Diagnostics.Internal;

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public class GaussDBTestHelpers : RelationalTestHelpers
{
    protected GaussDBTestHelpers() { }

    public static GaussDBTestHelpers Instance { get; } = new();

    public override IServiceCollection AddProviderServices(IServiceCollection services)
        => services.AddEntityFrameworkGaussDB();

    public override DbContextOptionsBuilder UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB(new GaussDBConnection("Host=localhost;Database=DummyDatabase"));

    public override LoggingDefinitions LoggingDefinitions { get; } = new GaussDBLoggingDefinitions();
}
