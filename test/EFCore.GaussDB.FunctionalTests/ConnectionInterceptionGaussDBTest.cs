using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.EntityFrameworkCore;

public abstract class ConnectionInterceptionGaussDBTestBase(ConnectionInterceptionGaussDBTestBase.InterceptionGaussDBFixtureBase fixture)
    : ConnectionInterceptionTestBase(fixture)
{
    [ConditionalTheory(Skip = "#2368")]
    public override Task Intercept_connection_creation_passively(bool async)
        => base.Intercept_connection_creation_passively(async);

    [ConditionalTheory(Skip = "#2368")]
    public override Task Intercept_connection_creation_with_multiple_interceptors(bool async)
        => base.Intercept_connection_creation_with_multiple_interceptors(async);

    [ConditionalTheory(Skip = "#2368")]
    public override Task Intercept_connection_to_override_connection_after_creation(bool async)
        => base.Intercept_connection_to_override_connection_after_creation(async);

    [ConditionalTheory(Skip = "#2368")]
    public override Task Intercept_connection_to_override_creation(bool async)
        => base.Intercept_connection_to_override_creation(async);

    public abstract class InterceptionGaussDBFixtureBase : InterceptionFixtureBase
    {
        protected override string StoreName
            => "ConnectionInterception";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<IInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkGaussDB(), injectedInterceptors);
    }

    protected override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB();

    protected override BadUniverseContext CreateBadUniverse(DbContextOptionsBuilder optionsBuilder)
        => new(optionsBuilder.UseGaussDB(new FakeDbConnection()).Options);

    public class FakeDbConnection : DbConnection
    {
        [AllowNull]
        public override string ConnectionString { get; set; }

        public override string Database
            => "Database";

        public override string DataSource
            => "DataSource";

        public override string ServerVersion
            => throw new NotImplementedException();

        public override ConnectionState State
            => ConnectionState.Closed;

        public override void ChangeDatabase(string databaseName)
            => throw new NotImplementedException();

        public override void Close()
            => throw new NotImplementedException();

        public override void Open()
            => throw new NotImplementedException();

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            => throw new NotImplementedException();

        protected override DbCommand CreateDbCommand()
            => throw new NotImplementedException();
    }

    public class ConnectionInterceptionGaussDBTest(ConnectionInterceptionGaussDBTest.InterceptionGaussDBFixture fixture)
        : ConnectionInterceptionGaussDBTestBase(fixture), IClassFixture<ConnectionInterceptionGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => false;
        }
    }

    public class ConnectionInterceptionWithDiagnosticsGaussDBTest(
        ConnectionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture fixture)
        : ConnectionInterceptionGaussDBTestBase(fixture),
            IClassFixture<ConnectionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => true;
        }
    }
}
