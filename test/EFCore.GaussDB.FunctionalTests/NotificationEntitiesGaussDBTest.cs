namespace Microsoft.EntityFrameworkCore;

public class NotificationEntitiesGaussDBTest(NotificationEntitiesGaussDBTest.NotificationEntitiesGaussDBFixture fixture)
    : NotificationEntitiesTestBase<NotificationEntitiesGaussDBTest.NotificationEntitiesGaussDBFixture>(fixture)
{
    public class NotificationEntitiesGaussDBFixture : NotificationEntitiesFixtureBase
    {
        protected override string StoreName { get; } = "NotificationEntities";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;
    }
}
