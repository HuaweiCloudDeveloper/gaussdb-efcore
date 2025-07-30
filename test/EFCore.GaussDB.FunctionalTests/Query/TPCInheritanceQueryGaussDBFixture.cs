namespace Microsoft.EntityFrameworkCore.Query;

public class TPCInheritanceQueryGaussDBFixture : TPCInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
