namespace Microsoft.EntityFrameworkCore;

public class MonsterFixupChangedChangingGaussDBTest(MonsterFixupChangedChangingGaussDBTest.MonsterFixupChangedChangingGaussDBFixture fixture)
    : MonsterFixupTestBase<MonsterFixupChangedChangingGaussDBTest.MonsterFixupChangedChangingGaussDBFixture>(fixture)
{
    public class MonsterFixupChangedChangingGaussDBFixture : MonsterFixupChangedChangingFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(
            ModelBuilder builder)
        {
            base.OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(builder);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(
                             e => e.GetProperties().Where(
                                 p =>
                                     p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))))
            {
                property.SetColumnType("timestamp without time zone");
            }

            builder.Entity<TMessage>().Property(e => e.MessageId).UseSerialColumn();
            builder.Entity<TProductPhoto>().Property(e => e.PhotoId).UseSerialColumn();
            builder.Entity<TProductReview>().Property(e => e.ReviewId).UseSerialColumn();
        }
    }
}
