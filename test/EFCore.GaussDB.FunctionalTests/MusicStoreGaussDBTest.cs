using Microsoft.EntityFrameworkCore.TestModels.MusicStore;

namespace Microsoft.EntityFrameworkCore;

public class MusicStoreGaussDBTest(MusicStoreGaussDBTest.MusicStoreGaussDBFixture fixture)
    : MusicStoreTestBase<MusicStoreGaussDBTest.MusicStoreGaussDBFixture>(fixture)
{
    public class MusicStoreGaussDBFixture : MusicStoreFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<CartItem>().Property(s => s.DateCreated).HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Order>().Property(s => s.OrderDate).HasColumnType("timestamp without time zone");
        }
    }
}
