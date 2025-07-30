using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore;

public abstract class CommandInterceptionGaussDBTestBase(CommandInterceptionGaussDBTestBase.InterceptionGaussDBFixtureBase fixture)
    : CommandInterceptionTestBase(fixture)
{
    public abstract class InterceptionGaussDBFixtureBase : InterceptionFixtureBase
    {
        protected override string StoreName
            => "CommandInterception";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<IInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkGaussDB(), injectedInterceptors);
    }

    public class CommandInterceptionGaussDBTest(CommandInterceptionGaussDBTest.InterceptionGaussDBFixture fixture)
        : CommandInterceptionGaussDBTestBase(fixture), IClassFixture<CommandInterceptionGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => false;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new GaussDBDbContextOptionsBuilder(base.AddOptions(builder))
                    .ExecutionStrategy(d => new GaussDBExecutionStrategy(d));
                return builder;
            }
        }
    }

    public class CommandInterceptionWithDiagnosticsGaussDBTest(
        CommandInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture fixture)
        : CommandInterceptionGaussDBTestBase(fixture), IClassFixture<CommandInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => true;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new GaussDBDbContextOptionsBuilder(base.AddOptions(builder))
                    .ExecutionStrategy(d => new GaussDBExecutionStrategy(d));
                return builder;
            }
        }
    }
}
