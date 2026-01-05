using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace Microsoft.EntityFrameworkCore;

public abstract class QueryExpressionInterceptionGaussDBTestBase(
    QueryExpressionInterceptionGaussDBTestBase.InterceptionGaussDBFixtureBase fixture)
    : QueryExpressionInterceptionTestBase(fixture)
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

    public class QueryExpressionInterceptionGaussDBTest(QueryExpressionInterceptionGaussDBTest.InterceptionGaussDBFixture fixture)
        : QueryExpressionInterceptionGaussDBTestBase(fixture), IClassFixture<QueryExpressionInterceptionGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override string StoreName
                => "QueryExpressionInterception";

            protected override bool ShouldSubscribeToDiagnosticListener
                => false;
        }
    }

    public class QueryExpressionInterceptionWithDiagnosticsGaussDBTest(
        QueryExpressionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture fixture)
        : QueryExpressionInterceptionGaussDBTestBase(fixture),
            IClassFixture<QueryExpressionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override string StoreName
                => "QueryExpressionInterceptionWithDiagnostics";

            protected override bool ShouldSubscribeToDiagnosticListener
                => true;
        }
    }
}
