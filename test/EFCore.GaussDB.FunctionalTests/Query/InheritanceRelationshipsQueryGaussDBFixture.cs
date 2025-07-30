namespace Microsoft.EntityFrameworkCore.Query;

public class InheritanceRelationshipsQueryGaussDBFixture : InheritanceRelationshipsQueryRelationalFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;
}
