using HuaweiCloud.EntityFrameworkCore.GaussDB.Design.Internal;

namespace Microsoft.EntityFrameworkCore;

public class DesignTimeGaussDBTest(DesignTimeGaussDBTest.DesignTimeGaussDBFixture fixture)
    : DesignTimeTestBase<DesignTimeGaussDBTest.DesignTimeGaussDBFixture>(fixture)
{
    protected override Assembly ProviderAssembly
        => typeof(GaussDBDesignTimeServices).Assembly;

    public class DesignTimeGaussDBFixture : DesignTimeFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
