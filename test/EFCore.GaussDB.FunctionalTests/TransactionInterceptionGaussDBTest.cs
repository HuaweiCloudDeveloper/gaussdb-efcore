namespace Microsoft.EntityFrameworkCore;

public abstract class TransactionInterceptionGaussDBTestBase(TransactionInterceptionGaussDBTestBase.InterceptionGaussDBFixtureBase fixture)
    : TransactionInterceptionTestBase(fixture)
{
    public abstract class InterceptionGaussDBFixtureBase : InterceptionFixtureBase
    {
        protected override string StoreName
            => "TransactionInterception";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<IInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkGaussDB(), injectedInterceptors);
    }

    public class TransactionInterceptionGaussDBTest(TransactionInterceptionGaussDBTest.InterceptionGaussDBFixture fixture)
        : TransactionInterceptionGaussDBTestBase(fixture), IClassFixture<TransactionInterceptionGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => false;
        }
    }

    public class TransactionInterceptionWithDiagnosticsGaussDBTest(
        TransactionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture fixture)
        : TransactionInterceptionGaussDBTestBase(fixture),
            IClassFixture<TransactionInterceptionWithDiagnosticsGaussDBTest.InterceptionGaussDBFixture>
    {
        public class InterceptionGaussDBFixture : InterceptionGaussDBFixtureBase
        {
            protected override bool ShouldSubscribeToDiagnosticListener
                => true;
        }
    }
}
