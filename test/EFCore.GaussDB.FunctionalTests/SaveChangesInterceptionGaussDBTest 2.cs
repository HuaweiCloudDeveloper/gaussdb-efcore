using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore;

public abstract class SaveChangesInterceptionGaussDBTestBase(SaveChangesInterceptionGaussDBTestBase.InterceptionGaussDBFixtureBase fixture)
    : SaveChangesInterceptionTestBase(fixture)
{
    public abstract class InterceptionGaussDBFixtureBase : InterceptionFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<IInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkGaussDB(), injectedInterceptors);

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            new GaussDBDbContextOptionsBuilder(base.AddOptions(builder))
                .ExecutionStrategy(d => new GaussDBExecutionStrategy(d));
            return builder;
        }
    }

    public class SaveChangesInterceptionGaussDBTest(SaveChangesInterceptionGaussDBTest.InterceptionGaussDBFixture fixture)
        : SaveChangesInterceptionGaussDBTestBase(fixture), IClassFixture<SaveChangesInterceptionGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override string StoreName
                => "SaveChangesInterception";

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

    public class SaveChangesInterceptionWithDiagnosticsGaussDBTest(
        SaveChangesInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture fixture)
        : SaveChangesInterceptionGaussDBTestBase(fixture),
            IClassFixture<SaveChangesInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override string StoreName
                => "SaveChangesInterceptionWithDiagnostics";

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
