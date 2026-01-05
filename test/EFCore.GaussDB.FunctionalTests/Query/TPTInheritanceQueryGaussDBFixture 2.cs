namespace Microsoft.EntityFrameworkCore.Query;

public class TPTInheritanceQueryGaussDBFixture : TPTInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
