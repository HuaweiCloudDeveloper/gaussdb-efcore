using HuaweiCloud.EntityFrameworkCore.GaussDB.Diagnostics.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore;

public class LoggingGaussDBTest : LoggingRelationalTestBase<GaussDBDbContextOptionsBuilder, GaussDBOptionsExtension>
{
    [Fact]
    public void Logs_context_initialization_admin_database()
        => Assert.Equal(
            ExpectedMessage($"AdminDatabase=foo {DefaultOptions}"),
            ActualMessage(s => CreateOptionsBuilder(s, b => ((GaussDBDbContextOptionsBuilder)b).UseAdminDatabase("foo"))));

    [Fact]
    public void Logs_context_initialization_postgres_version()
        => Assert.Equal(
            ExpectedMessage($"PostgresVersion=10.7 {DefaultOptions}"),
            ActualMessage(s => CreateOptionsBuilder(s, b => ((GaussDBDbContextOptionsBuilder)b).SetPostgresVersion(Version.Parse("10.7")))));

#pragma warning disable CS0618 // Authentication APIs on GaussDBDbContextOptionsBuilder are obsolete
    [Fact]
    public void Logs_context_initialization_provide_client_certificates_callback()
        => Assert.Equal(
            ExpectedMessage($"ProvideClientCertificatesCallback {DefaultOptions}"),
            ActualMessage(
                s => CreateOptionsBuilder(
                    s, b => ((GaussDBDbContextOptionsBuilder)b).ProvideClientCertificatesCallback(_ => { }))));

    [Fact]
    public void Logs_context_initialization_provide_password_callback()
        => Assert.Equal(
            ExpectedMessage($"ProvidePasswordCallback {DefaultOptions}"),
            ActualMessage(
                s => CreateOptionsBuilder(
                    s, b => ((GaussDBDbContextOptionsBuilder)b).ProvidePasswordCallback((_, _, _, _) => "password"))));

    [Fact]
    public void Logs_context_initialization_remote_certificate_validation_callback()
        => Assert.Equal(
            ExpectedMessage($"RemoteCertificateValidationCallback {DefaultOptions}"),
            ActualMessage(
                s => CreateOptionsBuilder(
                    s,
                    b => ((GaussDBDbContextOptionsBuilder)b).RemoteCertificateValidationCallback((_, _, _, _) => true))));
#pragma warning restore CS0618 // Authentication APIs on GaussDBDbContextOptionsBuilder are obsolete

    [Fact]
    public void Logs_context_initialization_reverse_null_ordering()
        => Assert.Equal(
            ExpectedMessage($"ReverseNullOrdering {DefaultOptions}"),
            ActualMessage(s => CreateOptionsBuilder(s, b => ((GaussDBDbContextOptionsBuilder)b).ReverseNullOrdering())));

    [Fact]
    public void Logs_context_initialization_user_range_definitions()
        => Assert.Equal(
            ExpectedMessage($"UserRangeDefinitions=[{typeof(int)}=>int4range] " + DefaultOptions),
            ActualMessage(s => CreateOptionsBuilder(s, b => ((GaussDBDbContextOptionsBuilder)b).MapRange<int>("int4range"))));

    protected override DbContextOptionsBuilder CreateOptionsBuilder(
        IServiceCollection services,
        Action<RelationalDbContextOptionsBuilder<GaussDBDbContextOptionsBuilder, GaussDBOptionsExtension>> relationalAction)
        => new DbContextOptionsBuilder()
            .UseInternalServiceProvider(services.AddEntityFrameworkGaussDB().BuildServiceProvider())
            .UseGaussDB("Data Source=LoggingGaussDBTest.db", relationalAction);

    protected override TestLogger CreateTestLogger()
        => new TestLogger<GaussDBLoggingDefinitions>();

    protected override string ProviderName
        => "HuaweiCloud.EntityFrameworkCore.GaussDB";

    protected override string ProviderVersion
        => typeof(GaussDBOptionsExtension).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!;
}
