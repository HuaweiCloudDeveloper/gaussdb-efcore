namespace Microsoft.EntityFrameworkCore.Query;

public class TPTInheritanceQueryGaussDBTest(TPTInheritanceQueryGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
    : TPTInheritanceQueryTestBase<TPTInheritanceQueryGaussDBFixture>(fixture, testOutputHelper)
{
    protected override bool EnforcesFkConstraints
        => !TestEnvironment.IsDistributed;
}
