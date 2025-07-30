namespace Microsoft.EntityFrameworkCore;

public class KeysWithConvertersGaussDBTest(KeysWithConvertersGaussDBTest.KeysWithConvertersGaussDBFixture fixture)
    : KeysWithConvertersTestBase<
        KeysWithConvertersGaussDBTest.KeysWithConvertersGaussDBFixture>(fixture)
{
    public class KeysWithConvertersGaussDBFixture : KeysWithConvertersFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => builder.UseGaussDB(b => b.MinBatchSize(1));
    }
}
